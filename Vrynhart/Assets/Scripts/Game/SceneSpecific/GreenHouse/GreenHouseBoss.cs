using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GreenHouseBoss : MonoBehaviour
{
    [SerializeField]
    string _uid;

    [SerializeField]
    string _bossName;

    [SerializeField]
    List<EnemyController> _plantTentacles;

    [SerializeField]
    List<TentacleSummonPair> _summonPoints;

    [SerializeField]
    EnemyController _summonEnemySource;

    [SerializeField]
    AudioClip _music;

    EnemyController _currentTentacle;
    bool _fightStarted = false;
    int _turnsPassed = 0;
    int _tentacleCount, _startingTentacleCount;
    int _maxHealth = 30;
    int Health => (int)(_maxHealth * (_tentacleCount / (float)_startingTentacleCount));

    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        if (saveData.CompletedFlags.Contains(_uid))
        {
            CleanUpTentacles();
            CloseEyes();
            Destroy(gameObject);
        }
    }

    async void OnTriggerEnter2D(Collider2D collision)
    {
        if (_fightStarted)
            return;

        _startingTentacleCount = _tentacleCount = _plantTentacles.Count;
        _plantTentacles.ForEach(p => p.gameObject.SetActive(false));
        _currentTentacle = _plantTentacles.First(p => p != null && p.gameObject.activeSelf == false);
        _currentTentacle.gameObject.SetActive(true);

        // display health bar
        await BossUI.Show(_bossName, new BossUI.BossDisplayData() { Hp = Health, MaxHp = _maxHealth });

        // start music
        Brokers.Audio.Publish(new MusicEvent(_music, 1.5f));

        // set up enemy controller listening
        Brokers.Default.Receive<EnemyDiedEvent>()
            .Where(e => _plantTentacles.Contains(e.EnemyController))
            .Subscribe(OnTentacleDied)
            .AddTo(this);

        Brokers.Default.Receive<TurnProgressionEvent>()
            .Subscribe(e =>
            {
                ++_turnsPassed;
                if (_turnsPassed >= 9)
                {
                    Summon();
                    _turnsPassed = 0;
                }
            })
            .AddTo(this);

        _fightStarted = true;
    }

    void CleanUpTentacles()
    {
        _plantTentacles.ForEach(t => Destroy(t.gameObject));
    }

    void OnTentacleDied(EnemyDiedEvent e)
    {
        --_tentacleCount;

        if (_tentacleCount <= 0) // boss is dead
        {
            // stop the music
            Brokers.Audio.Publish(new MusicEvent(null, shouldFade: false));

            // close the ui
            BossUI.Close();

            // close eyes
            CloseEyes();

            // save the win
            GameSaveSystem.CacheGame(_uid);

            // destroy this
            Destroy(gameObject);
        }
        else
        {
            _currentTentacle = _plantTentacles.First(p => p != null && p.gameObject.activeSelf == false);
            _currentTentacle.gameObject.SetActive(true);
            Brokers.Default.Publish(new BossUI.BossDisplayData() { Hp = Health, MaxHp = _maxHealth });
        }
    }

    void Summon()
    {
        var count = FindObjectsOfType<EnemyController>()
            .Where(e => e.name.Contains(_summonEnemySource.name))
            .Count();
        if (count >= 3)
            return;

        var player = FindObjectOfType<PlayerController>();
        if (player == null)
            return;

        var summonPoint = _summonPoints.First(s => s.tentacle == _currentTentacle);
        if (summonPoint != null)
        {
            var enemy = Instantiate(_summonEnemySource);
            enemy.transform.position = summonPoint.summon.position;
            var moveToward = enemy.GetComponent<MoveToward>();
            if (moveToward)
                moveToward.Target = player.transform;
        }
    }

    void CloseEyes()
    {
        var eyes = FindObjectsOfType<DemonicEyeController>();
        foreach (var eye in eyes)
            eye.Close();
    }
}

[System.Serializable]
public class TentacleSummonPair
{
    public EnemyController tentacle;
    public Transform summon;
}