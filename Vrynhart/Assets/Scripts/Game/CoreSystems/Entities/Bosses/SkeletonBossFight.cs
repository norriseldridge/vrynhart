using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.Experimental.Rendering.Universal;

public class SkeletonBossFight : MonoBehaviour
{
    [SerializeField]
    string _persistentId;

    [Header("Intro")]
    [SerializeField]
    Light2D _light;

    [Header("Boss")]
    [SerializeField]
    string _bossName;

    [SerializeField]
    BossUI.BossDisplayData _data;

    [SerializeField]
    GameObject _boss;

    [SerializeField]
    SkeletonSkullExplosition _bossDeath;

    [SerializeField]
    AudioClip _bossDeathSfx;

    [SerializeField]
    float _bossDeathVolume;

    [SerializeField]
    float _bossDeathPitch;

    [Header("Skeleton Army Spawning")]
    [SerializeField]
    EnemyController _source;

    [SerializeField]
    List<EnemyController> _spawns;

    [SerializeField]
    int _spawnTurnDelay;

    [SerializeField]
    int _maxSpawns;

    [Header("Audio")]
    [SerializeField]
    AudioClip _bossMusic;

    [SerializeField]
    float _volume;

    [SerializeField]
    AudioClip _victoryMusic;

    bool _fightStarted = false;
    int _turns = 0;
    int _levelMonsters;
    int _spawnedCount = 0;

    void Start()
    {
        var save = GameSaveSystem.GetCachedSaveData();
        HandleBossPreviouslyCompleted(save);
        Brokers.Default.Receive<SaveDataChangeEvent>()
            .Subscribe(e => HandleBossPreviouslyCompleted(e.SaveData))
            .AddTo(this);
    }

    void HandleBossPreviouslyCompleted(SaveData save)
    {
        if (save.CompletedFlags.Contains(_persistentId))
        {
            // destory the boss
            Destroy(_boss.gameObject);

            // close the eyes
            foreach (var eye in FindObjectsOfType<DemonicEyeController>())
                eye.Close();

            // destory the spawns
            foreach (var spawn in _spawns)
            {
                if (spawn != null)
                    Destroy(spawn.gameObject);
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_fightStarted)
            return;

        var player = collision.GetComponent<PlayerController>();
        if (player != null)
            StartFight();
    }

    void Update()
    {
        if (!_fightStarted)
            return;

        if (_spawns.All(s => s == null))
        {
            // boss defeated
            enabled = false;
            EndFight();
        }
        else
        {
            _data.Hp = _spawns.Count(s => s != null) * 10;
            Brokers.Default.Publish(new BossUI.BossDisplayData() { Hp = _data.Hp, MaxHp = _data.MaxHp });
        }
    }

    async void StartFight()
    {
        if (_fightStarted)
            return;

        _levelMonsters = FindObjectsOfType<EnemyController>().Length;
        _boss.gameObject.SetActive(true);
        _fightStarted = true;

        Brokers.Audio.Publish(new MusicEvent(_bossMusic, _volume));
        _ = StartCoroutine(FadeInLight());

        await BossUI.Show(_bossName, new BossUI.BossDisplayData() { Hp = _data.Hp, MaxHp = _data.MaxHp });

        Brokers.Default.Receive<TurnProgressionEvent>()
            .Subscribe(_ =>
            {
                if (_spawnedCount >= _maxSpawns)
                    return;

                ++_turns;
                if (_turns >= _spawnTurnDelay)
                {
                    _turns = 0;
                    SpawnEnemy();
                }
            })
            .AddTo(this);
    }

    IEnumerator FadeInLight()
    {
        var target = 0.8f;
        for (var i = 0.0f; i < target; i += Time.deltaTime)
        {
            _light.intensity = i;
            yield return null;
        }
        _light.intensity = target;
    }

    void SpawnEnemy()
    {
        var possible = _spawns.Where(s => s != null).ToArray();
        if (possible.Length == 0)
            return;

        var r = Random.Range(0, possible.Length);
        var location = possible[r];
        var enemy = Instantiate(_source, location.transform.position, Quaternion.Euler(0, 0, 0));
        var move = enemy.GetComponent<MoveToward>();
        if (move != null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
                move.Target = player.transform;
        }

        // calculate how many spawns are out there
        _spawnedCount = FindObjectsOfType<EnemyController>().Length - _levelMonsters;
    }

    void EndFight()
    {
        BossUI.Close();

        // destroy the boss
        Brokers.Audio.Publish(new AudioEvent(_bossDeathSfx, _bossDeathVolume, _bossDeathPitch, _bossDeathPitch));
        Destroy(_boss);
        _bossDeath.gameObject.SetActive(true);
        _bossDeath.Explode();

        // close the eyes
        foreach (var eye in FindObjectsOfType<DemonicEyeController>())
            eye.Close();

        // stop music
        Brokers.Audio.Publish(new MusicEvent(null));
        Brokers.Audio.Publish(new AudioEvent(_victoryMusic));
        StartCoroutine(RetartLevelMusic());

        // mark that this boss is done
        GameSaveSystem.CacheGame(_persistentId);

        // destory the boss logic
        Destroy(gameObject);
    }

    IEnumerator RetartLevelMusic()
    {
        yield return new WaitForSeconds(4);
        Brokers.Audio.Publish(new RestartLevelMusicEvent());
    }
}
