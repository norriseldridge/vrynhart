using UnityEngine;
using UniRx;

public class DemonBoss : MonoBehaviour
{
    [SerializeField]
    string _uid;

    [SerializeField]
    string _bossName;

    [SerializeField]
    EnemyController _boss;

    [SerializeField]
    AudioClip _music;

    [SerializeField]
    AudioClip _victoryMusic;

    [SerializeField]
    AudioClip _screech;

    [SerializeField]
    float _screechVolume;

    [SerializeField]
    AudioClip[] _fightGrunts;

    [SerializeField]
    float _gruntVolume;

    [SerializeField]
    float _gruntDelay;

    bool _triggered = false;
    int _startingHealth;
    float _currentGruntDelay;

    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        if (saveData.CompletedFlags.Contains(_uid))
        {
            Destroy(_boss.gameObject);
            Destroy(gameObject);

            // close the eyes
            var eyes = FindObjectsOfType<DemonicEyeController>();
            foreach (var eye in eyes)
                eye.Close();
        }
    }

    async void OnTriggerEnter2D(Collider2D collision)
    {
        // start boss fight!
        if (!_triggered)
        {
            _boss.enabled = true;

            _startingHealth = _boss.Health;
            await BossUI.Show(_bossName, new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });

            Brokers.Audio.Publish(new MusicEvent(_music));
            Brokers.Audio.Publish(new AudioEvent(_screech, _screechVolume));
            Brokers.Default.Publish(new CameraShakeEvent(0.5f, 0.05f, 36));

            // set up enemy controller listening
            Brokers.Default.Receive<EnemyTakeDamageEvent>()
                .Where(e => e.EnemyController == _boss)
                .Subscribe(e => {
                    Brokers.Default.Publish(new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });
                })
                .AddTo(this);

            Brokers.Default.Receive<EnemyDiedEvent>()
                .Where(e => e.EnemyController == _boss)
                .Subscribe(e => {
                    // stop the music
                    Brokers.Audio.Publish(new MusicEvent(null));
                    Brokers.Audio.Publish(new AudioEvent(_victoryMusic));

                    // close the ui
                    BossUI.Close();

                    // save the win
                    GameSaveSystem.CacheGame(_uid);

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
            Brokers.Audio.Publish(new AudioEvent(_fightGrunts[i], _gruntVolume));
        }
    }
}
