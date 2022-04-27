using UnityEngine;

public class Lever : RequiresPrompt
{
    [SerializeField]
    string _leverId;

    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    Sprite _on;

    [SerializeField]
    AudioClip _sfx;

    [SerializeField]
    float _volume;

    bool _pulled = false;

    void Start()
    {
        var data = GameSaveSystem.GetCachedSaveData();
        if (data.CompletedFlags.Contains(_leverId))
        {
            _pulled = true;
            Brokers.Default.Publish(new LeverEvent(_leverId, true));
        }
    }

    void Update()
    {
        if (_pulled)
            return;

        if (PromptUser != null && PromptUser.AcceptedPrompt)
        {
            _pulled = true;
            _renderer.sprite = _on;
            Brokers.Audio.Publish(new AudioEvent(_sfx, _volume));
            Brokers.Default.Publish(new LeverEvent(_leverId));
            GameSaveSystem.CacheGame(_leverId);
        }
    }
}
