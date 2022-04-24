using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DravenFight : MonoBehaviour
{
    [SerializeField]
    string _persistentId;

    [Header("Boss")]
    [SerializeField]
    string _bossName;

    [SerializeField]
    EnemyController _boss;

    [SerializeField]
    ConversationPrompt _startFightPrompt;

    [SerializeField]
    ConversationPrompt _lastWordsPrompt;

    [Header("Progression")]
    [SerializeField]
    List<Tile> _toEnable;

    [SerializeField]
    GameObject _barrier;

    [Header("Audio")]
    [SerializeField]
    AudioClip _bossMusic;

    [SerializeField]
    float _volume;

    bool _triggered = false;
    int _startingHealth;

    // Start is called before the first frame update
    void Start()
    {
        var save = GameSaveSystem.GetCachedSaveData();
        if (save.CompletedFlags.Contains(_persistentId))
        {
            _toEnable.ForEach(e => e.enabled = true);
            Destroy(_barrier);
            Destroy(_boss.gameObject);
            Destroy(gameObject);
        }
        else
        {
            _toEnable.ForEach(e => e.enabled = false);
            _boss.enabled = false;
            _lastWordsPrompt.enabled = false;

            Brokers.Default.Receive<SimpleConversationEvent>()
                .Where(e => e.Prompt.gameObject == gameObject)
                .Subscribe(_ => Brokers.Audio.Publish(new MusicEvent(null)))
                .AddTo(this);

            Brokers.Default.Receive<ConversationCompleteEvent>()
                .Where(e => e.Prompt.gameObject == gameObject)
                .Subscribe(_ => StartFight())
                .AddTo(this);
        }
    }

    async void StartFight()
    {
        if (_triggered)
            return;

        _triggered = true;

        // no more talking
        _startFightPrompt.enabled = false;
        Brokers.Default.Publish(new ExitPromptEvent());

        // show the boss health bar
        _startingHealth = _boss.Health;
        await BossUI.Show(_bossName, new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth });

        // start boss music
        Brokers.Audio.Publish(new MusicEvent(_bossMusic, shouldFade: false));

        // enable to boss
        _boss.enabled = true;

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

                // close the ui
                BossUI.Close();

                // start the final words converstation
                Brokers.Default.Publish(new SimpleConversationEvent(_lastWordsPrompt));

                // save the win
                GameSaveSystem.CacheGame(_persistentId);

                // enable the tiles
                _toEnable.ForEach(e => e.enabled = true);
                Destroy(_barrier);
            })
            .AddTo(this);
    }
}
