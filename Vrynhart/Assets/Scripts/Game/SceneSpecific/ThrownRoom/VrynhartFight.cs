using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

public class VrynhartFight : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField]
    string _bossName;

    [SerializeField]
    EnemyController _boss;

    [SerializeField]
    ConversationPrompt _startFightPrompt;

    [SerializeField]
    List<EnemyController> _fires;

    [SerializeField]
    List<EnemyController> _vampires;

    [SerializeField]
    List<LevelItem> _items;

    [Header("Outro")]
    [SerializeField]
    Image _fadeOut;

    [Header("Audio")]
    [SerializeField]
    AudioClip _music;

    [SerializeField]
    AudioClip _victoryMusic;

    bool _triggered = false;
    int _startingHealth;
    int _state = 0;

    void Start()
    {
        // disable all items
        _items.ForEach(i => i.gameObject.SetActive(false));

        Brokers.Default.Receive<ConversationCompleteEvent>()
            .Where(e => e.Prompt.gameObject == gameObject)
            .Subscribe(_ => StartFight())
            .AddTo(this);
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
        Brokers.Audio.Publish(new MusicEvent(_music, shouldFade: false));

        // start listening for turns
        Brokers.Default.Receive<TurnProgressionEvent>()
            .Subscribe(OnTurnProgressionEvent)
            .AddTo(this);

        // health reporting
        Brokers.Default.Receive<EnemyTakeDamageEvent>()
            .Where(e => e.EnemyController == _boss)
            .Subscribe(_ => Brokers.Default.Publish(new BossUI.BossDisplayData() { Hp = _boss.Health, MaxHp = _startingHealth }))
            .AddTo(this);

        Brokers.Default.Receive<EnemyDiedEvent>()
            .Where(e => e.EnemyController == _boss)
            .Subscribe(e => {
                // slow-mo
                Time.timeScale = 0.6f;

                // shake the camera
                Brokers.Default.Publish(new CameraShakeEvent(10, 0.5f, 7));

                // stop the music
                Brokers.Audio.Publish(new MusicEvent(null));
                Brokers.Audio.Publish(new AudioEvent(_victoryMusic));

                // close the ui
                BossUI.Close();

                // load the end scene
                StartCoroutine(FadeOut());
            })
            .AddTo(this);
    }

    void OnTurnProgressionEvent(TurnProgressionEvent e)
    {
        if (_state == 0)
        {
            _vampires.ForEach(v => v.enabled = true);
        }

        if (_state > 1)
        {
            _fires[0].TileMover.TryMove(Vector2.left);
            _fires[3].TileMover.TryMove(Vector2.right);
        }

        if (_state > 4)
        {
            _fires[1].TileMover.TryMove(Vector2.left);
            _fires[4].TileMover.TryMove(Vector2.right);
        }

        if (_state > 8)
        {
            _fires[2].TileMover.TryMove(Vector2.left);
            _fires[5].TileMover.TryMove(Vector2.right);
        }

        if (_state == 22)
        {
            _state = 0;

            // reset positions
            _fires[0].transform.position += Vector3.right * 12;
            _fires[1].transform.position += Vector3.right * 12;
            _fires[2].transform.position += Vector3.right * 12;

            _fires[3].transform.position += Vector3.left * 12;
            _fires[4].transform.position += Vector3.left * 12;
            _fires[5].transform.position += Vector3.left * 12;
        }

        // "drop" a random item
        if (_state % 7 == 0)
        {
            if (_items.Count > 0)
            {
                var ri = _items[Random.Range(0, _items.Count)];
                ri.gameObject.SetActive(true);
                _items.Remove(ri);
            }
        }

        ++_state;
    }

    IEnumerator FadeOut()
    {
        _fadeOut.enabled = true;
        float time = 2.5f;
        for (var i = 0.0f; i < time; i += Time.deltaTime)
        {
            _fadeOut.color = new Color(1, 1, 1, i / time);
            yield return null;
        }

        Time.timeScale = 1;

        // load ending scene
        SceneManager.LoadScene(Constants.Game.OutroScene);
    }
}
