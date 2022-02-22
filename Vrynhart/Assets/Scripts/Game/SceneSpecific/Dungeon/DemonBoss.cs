using UnityEngine;
using UniRx;

public class DemonBoss : MonoBehaviour
{
    [SerializeField]
    string _bossName;

    [SerializeField]
    EnemyController _boss;

    [SerializeField]
    AudioClip _music;

    [SerializeField]
    AudioClip _screech;

    [SerializeField]
    float _screechVolume;

    [SerializeField]
    AudioClip[] _fightGrunts;

    [SerializeField]
    float _gruntDelay;

    bool _triggered = false;
    int _startingHealth;
    float _currentGruntDelay;

    async void OnTriggerEnter2D(Collider2D collision)
    {
        // start boss fight!
        if (!_triggered)
        {
            _startingHealth = _boss.Health;
            await BossUI.Show(_bossName, new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });

            MessageBroker.Default.Publish(new MusicEvent(_music));
            MessageBroker.Default.Publish(new AudioEvent(_screech, _screechVolume));

            MessageBroker.Default.Receive<EnemyTakeDamageEvent>()
                .Where(e => e.EnemyController == _boss)
                .Subscribe(e => {
                    MessageBroker.Default.Publish(new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });
                })
                .AddTo(this);

            MessageBroker.Default.Receive<EnemyDiedEvent>()
                .Where(e => e.EnemyController == _boss)
                .Subscribe(e => {
                    // stop the music
                    MessageBroker.Default.Publish(new MusicEvent(null));

                    // close the ui
                    BossUI.Close();

                    // close the eyes
                    var eyes = FindObjectsOfType<DemonicEyeController>();
                    foreach (var eye in eyes)
                        eye.Close();
                })
                .AddTo(this);

            _triggered = true;

            _currentGruntDelay = _gruntDelay;
        }
    }

    void Update()
    {
        if (!_triggered)
            return;

        if (_boss == null || _boss.IsDead)
            return;

        // play screeches
        _currentGruntDelay -= Time.deltaTime;
        if (_currentGruntDelay <= 0)
        {
            _currentGruntDelay = Random.Range(_gruntDelay / 2, _gruntDelay);
            var i = Random.Range(0, _fightGrunts.Length);
            MessageBroker.Default.Publish(new AudioEvent(_fightGrunts[i], _screechVolume));
        }
    }
}
