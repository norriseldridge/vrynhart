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

    [SerializeField]
    List<Tile> _tilesToDisableAtStart;

    [SerializeField]
    List<Tile> _tilesToEnableAtEnd;

    [Header("Boss")]
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
    List<CellarHeadstone> _spawns;

    [SerializeField]
    int _spawnTurnDelay;

    [SerializeField]
    int _maxSpawns;

    [Header("Audio")]
    [SerializeField]
    AudioClip _bossMusic;

    [SerializeField]
    float _volume;

    bool _fightStarted = false;
    int _turns = 0;
    int _levelMonsters;
    int _spawnedCount = 0;

    void Start()
    {
        var save = GameSaveSystem.GetCachedSaveData();
        HandleBossPreviouslyCompleted(save);
        MessageBroker.Default.Receive<SaveDataChangeEvent>()
            .Subscribe(e => HandleBossPreviouslyCompleted(e.SaveData))
            .AddTo(this);
    }

    void HandleBossPreviouslyCompleted(SaveData save)
    {
        if (save.CompletedFlags.Contains(_persistentId))
        {
            // enable the tiles
            foreach (var tile in _tilesToEnableAtEnd)
                tile.enabled = true;

            // destory the boss
            Destroy(_boss);

            // destory the headstones
            foreach (var headstone in _spawns)
                headstone.MarkDestroyed();

            enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
            return;

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

        if (_spawns.All(s => s.Destroyed))
        {
            // boss defeated
            enabled = false;
            EndFight();
        }
    }

    void StartFight()
    {
        _levelMonsters = FindObjectsOfType<EnemyController>().Length;
        _boss.gameObject.SetActive(true);
        _fightStarted = true;

        foreach (var tile in _tilesToDisableAtStart)
            tile.enabled = false;

        MessageBroker.Default.Publish(new MusicEvent(_bossMusic, _volume));
        StartCoroutine(FadeInLight());

        MessageBroker.Default.Receive<TurnProgressionEvent>()
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
        for (var i = 0.0f; i < 0.85f; i += Time.deltaTime)
        {
            _light.intensity = i;
            yield return null;
        }
    }

    void SpawnEnemy()
    {
        var r = Random.Range(0, _spawns.Count);
        var location = _spawns[r];

        if (location.Destroyed)
            return;

        var enemy = Instantiate(_source, location.transform.position + Vector3.down, Quaternion.Euler(0, 0, 0));
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
        // enable the tiles again so you can leave
        foreach (var tile in _tilesToEnableAtEnd)
            tile.enabled = true;

        // destroy the boss
        MessageBroker.Default.Publish(new AudioEvent(_bossDeathSfx, _bossDeathVolume, _bossDeathPitch, _bossDeathPitch));
        Destroy(_boss);
        _bossDeath.gameObject.SetActive(true);
        _bossDeath.Explode();

        // stop music
        MessageBroker.Default.Publish(new MusicEvent(null));
        StartCoroutine(RetartLevelMusic());

        // mark that this boss is done
        GameSaveSystem.CacheGame(_persistentId);
    }

    IEnumerator RetartLevelMusic()
    {
        yield return new WaitForSeconds(4);
        MessageBroker.Default.Publish(new RestartLevelMusicEvent());
    }
}
