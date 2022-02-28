using UnityEngine;
using UniRx;
using UnityEngine.Experimental.Rendering.Universal;

public class CellarBoss : MonoBehaviour
{
    [SerializeField]
    string _persistentId;

    [SerializeField]
    EnemyController _boss;

    [SerializeField]
    string _bossName;

    [SerializeField]
    AudioClip _music;

    [SerializeField]
    GiantSlugTentacle _source;

    [SerializeField]
    EnemyController _slugSource;

    [SerializeField]
    Transform[] _slugSummonLocations;

    [SerializeField]
    Tile _blockingTile;

    [SerializeField]
    Light2D[] _lights;

    bool _triggered = false;
    int _startingHealth;

    int _phase = 0;
    int _turnDelay = 0;

    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        if (saveData.CompletedFlags.Contains(_persistentId))
        {
            enabled = false;
            _blockingTile.IsFloor = true;
            foreach (var light in _lights)
                light.enabled = false;
            foreach (var eye in FindObjectsOfType<DemonicEyeController>())
                eye.gameObject.SetActive(false);
            Destroy(_boss.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
            return;

        if (_triggered)
            return;

        var player = collision.GetComponent<PlayerController>();
        if (player != null)
            StartFight();
    }

    async void StartFight()
    {
        _triggered = true;

        // start the music
        MessageBroker.Default.Publish(new MusicEvent(_music, 1, false));

        // show the boss ui
        _startingHealth = _boss.Health;
        await BossUI.Show(_bossName, new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });

        // listen for health changes
        MessageBroker.Default.Receive<EnemyTakeDamageEvent>()
            .Where(e => e.EnemyController == _boss)
            .Subscribe(OnEnemyTakeDamageEvent)
            .AddTo(this);

        MessageBroker.Default.Receive<EnemyDiedEvent>()
            .Where(e => e.EnemyController == _boss)
            .Subscribe(OnEnemyDiedEvent)
            .AddTo(this);

        // each turn
        MessageBroker.Default.Receive<TurnProgressionEvent>()
            .Subscribe(OnTurnProgressionEvent)
            .AddTo(this);
    }

    void OnEnemyTakeDamageEvent(EnemyTakeDamageEvent e)
    {
        MessageBroker.Default.Publish(new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });

        if (_boss.Health > 25)
            _phase = 0;
        if (_boss.Health > 20 && _phase == 0)
            _phase = 1;
        else if (_boss.Health > 10 && _phase == 1)
            _phase = 2;
        else
            _phase = 3;
    }

    void OnEnemyDiedEvent(EnemyDiedEvent e)
    {
        // resume the normal music the music
        MessageBroker.Default.Publish(new RestartLevelMusicEvent());

        GameSaveSystem.CacheGame(_persistentId);

        BossUI.Close();

        enabled = false;

        // kill the lights
        foreach (var light in _lights)
            light.enabled = false;

        // close the eyes
        foreach (var eye in FindObjectsOfType<DemonicEyeController>())
            eye.Close();

        // enable the blocked tile
        _blockingTile.IsFloor = true;
    }

    void OnTurnProgressionEvent(TurnProgressionEvent e)
    {
        if (!enabled)
            return;

        var player = FindObjectOfType<PlayerController>();
        if (player == null)
            return;

        switch (_phase)
        {
            case 0:
                if (_turnDelay == 4)
                {
                    SpawnTentacle(player.transform.position);
                    _turnDelay = 0;
                }
                break;

            case 1:
                if (_turnDelay == 3)
                {
                    SpawnTentacle(player.transform.position);
                    _turnDelay = 0;
                }
                break;

            case 2:
                if (_turnDelay == 4)
                {
                    SpawnTentacle(player.transform.position);
                    SummonSlug(player);
                    _turnDelay = 0;
                }
                break;

            case 3: // wait two turns, the spawn two
                if (_turnDelay == 2 || _turnDelay == 3)
                    SpawnTentacle(player.transform.position);
                else if (_turnDelay == 7)
                    _turnDelay = 0;
                break;
        }

        ++_turnDelay;
    }

    void SpawnTentacle(Vector3 position)
    {
        var tentacle = Instantiate(_source);
        tentacle.transform.position = position;
    }

    void SummonSlug(PlayerController player, int index = -1)
    {
        var slug = Instantiate(_slugSource);
        var location = _slugSummonLocations[index >= 0 ? index : Random.Range(0, _slugSummonLocations.Length)];
        slug.transform.position = location.position;
        slug.GetComponent<SlugLogic>().Player = player;
    }
}