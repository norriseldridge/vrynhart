using System.Linq;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public class WerewolfBossLogic : MonoBehaviour
{
    [SerializeField]
    string _uid;

    [SerializeField]
    string _bossName;

    [SerializeField]
    EnemyController _boss;

    [SerializeField]
    Transform[] _summonPoints;

    [SerializeField]
    EnemyController _summonEnemySource;

    [SerializeField]
    AudioClip _startSfx;

    [SerializeField]
    AudioClip _music;

    [SerializeField]
    AudioClip _victoryMusic;

    int _startingHealth;
    bool _fightStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        if (saveData.CompletedFlags.Contains(_uid))
        {
            Destroy(_boss.gameObject);
            Destroy(gameObject);
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
        Brokers.Audio.Publish(new AudioEvent(_startSfx, 0.5f));
        Brokers.Audio.Publish(new MusicEvent(_music, 1.5f));

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
                    Brokers.Audio.Publish(new MusicEvent(null, shouldFade: false));
                    Brokers.Audio.Publish(new AudioEvent(_victoryMusic));

                    // close the ui
                    BossUI.Close();

                    // save the win
                    GameSaveSystem.CacheGame(_uid);
                })
                .AddTo(this);

        Brokers.Default.Receive<WerewolfSummonEvent>()
            .Subscribe(OnSummon)
            .AddTo(this);

        _boss.enabled = true;
        _fightStarted = true;
    }

    void OnSummon(WerewolfSummonEvent e)
    {
        var count = FindObjectsOfType(_summonEnemySource.GetType()).Length;
        if (count >= 3)
            return; // don't spawn more than 3

        var player = FindObjectOfType<PlayerController>();
        if (player == null)
            return;

        var summonPoint = _summonPoints.OrderBy(p => Vector3.Distance(p.position, e.Position)).FirstOrDefault();
        if (summonPoint)
        {
            var enemy = Instantiate(_summonEnemySource);
            enemy.transform.position = summonPoint.position;
            var moveToward = enemy.GetComponent<MoveToward>();
            if (moveToward)
                moveToward.Target = player.transform;
        }
    }
}
