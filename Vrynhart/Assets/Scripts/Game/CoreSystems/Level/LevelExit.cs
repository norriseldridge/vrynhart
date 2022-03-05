using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    [SerializeField]
    string _nextLevel;

    [SerializeField]
    string _startingPointIdentifier;

    [SerializeField]
    bool _requiresPrompt = true;

    [Header("Audio")]
    [SerializeField]
    AudioClip _sfx;

    [SerializeField]
    float _volume;

    public string NextLevel => _nextLevel;
    public string StartingPoint => _startingPointIdentifier;

    LevelExiter _exiter;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
            return;

        var exiter = collision.gameObject.GetComponent<LevelExiter>();
        if (exiter && exiter.enabled)
        {
            _exiter = exiter;
            if (_requiresPrompt)
                Brokers.Default.Publish(new EnterPromptEvent());
            else
                ExitLevel();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled)
            return;

        var exiter = collision.gameObject.GetComponent<LevelExiter>();
        if (exiter && exiter.enabled)
        {
            _exiter = null;
            Brokers.Default.Publish(new ExitPromptEvent());
        }
    }

    void Update()
    {
        if (_exiter != null && _exiter.enabled && _exiter.RequestedExit)
        {
            ExitLevel();
        }
    }

    async void ExitLevel()
    {
        enabled = false;
        _exiter.enabled = false;

        // cache the player to be loaded in the next scene
        var player = FindObjectOfType<PlayerController>();
        GameSaveSystem.CacheGame(player);

        // stop the player and stop any turns from happening
        player.enabled = false;
        Brokers.Default.Publish(new StopTurnsEvent());

        // play the sfx
        Brokers.Audio.Publish(new AudioEvent(_sfx, _volume));

        // fade out
        await TransitionController.TriggerTransitionAsTask();

        // load the scene
        LevelInitializer.StartingPositionIdentifier = _startingPointIdentifier;
        SceneManager.LoadSceneAsync(_nextLevel);
    }
}
