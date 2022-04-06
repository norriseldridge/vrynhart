using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public class BatBoss : MonoBehaviour
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
    List<Transform> _bossLocations;

    [SerializeField]
    List<EnemyController> _enemiesToSpawn;

    [SerializeField]
    List<Transform> _spawnLocations;

    [SerializeField]
    Swirl _swirlFxSource;

    int _startingHealth;
    bool _fightStarted = false;
    IReactiveProperty<int> _bossState = new ReactiveProperty<int>(0);

    void Start()
    {
        _enemiesToSpawn.ForEach(e => e.enabled = false);

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
        if (_fightStarted)
            return;

        // display health bar
        _startingHealth = _boss.Health;
        await BossUI.Show(_bossName, new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });

        // start music
        Brokers.Audio.Publish(new MusicEvent(_music));

        // set up enemy controller listening
        Brokers.Default.Receive<EnemyTakeDamageEvent>()
            .Where(e => e.EnemyController == _boss)
            .Subscribe(e => {
                Brokers.Default.Publish(new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });

                if (_bossState.Value == 0 && _boss.Health <= 25)
                    _bossState.Value = 1;
                else if (_bossState.Value == 1 && _boss.Health <= 20)
                    _bossState.Value = 2;
                else if (_bossState.Value == 2 && _boss.Health <= 15)
                    _bossState.Value = 3;
                else if (_bossState.Value == 3 && _boss.Health <= 10)
                    _bossState.Value = 4;
                else if (_bossState.Value == 4 && _boss.Health <= 5)
                    _bossState.Value = 5;
            })
            .AddTo(this);

        Brokers.Default.Receive<EnemyDiedEvent>()
                .Where(e => e.EnemyController == _boss)
                .Subscribe(e => {
                    // stop the music
                    Brokers.Audio.Publish(new MusicEvent(null));

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

        _bossState
            .Subscribe(HandleBossState)
            .AddTo(this);

        _fightStarted = true;
    }

    void HandleBossState(int state)
    {
        if (state == 0)
            return;

        MoveBoss();
        SpawnVampire();
    }

    async void MoveBoss()
    {
        _boss.enabled = false; // stop spam click to kill
        var fx = Instantiate(_swirlFxSource);
        fx.transform.position = _boss.transform.position;
        await StartCoroutine(Fade(fx, 0, 1));

        var closest = _bossLocations.OrderBy(t => Vector3.Distance(_boss.transform.position, t.position)).First();
        var possible = _bossLocations.Where(t => t != closest).ToArray();
        var index = Random.Range(0, possible.Count());
        _boss.transform.position = possible[index].position;

        await StartCoroutine(Fade(fx, 1, 0));
        Destroy(fx.gameObject);
        _boss.enabled = true; // enable killing again
    }

    IEnumerator Fade(Swirl fx, float start, float target)
    {
        yield return fx.FadeAlpha(start, target, 1.5f);
    }

    void SpawnVampire()
    {
        if (_enemiesToSpawn.Count > 0)
        {
            var enemy = _enemiesToSpawn[0];
            var pos = _spawnLocations[Random.Range(0, _spawnLocations.Count)].position;
            enemy.transform.position = pos;
            enemy.enabled = true;
            _enemiesToSpawn.RemoveAt(0);
        }
    }
}
