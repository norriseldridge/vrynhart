using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public class Chest : MonoBehaviour
{
    [Header("Data")]
    [SerializeField]
    string _uid;

    [SerializeField]
    string _itemId;

    [SerializeField]
    int _count;

    [Header("Visuals")]
    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    Sprite _open;

    [SerializeField]
    Sprite _close;

    [SerializeField]
    AudioClip _sfx;

    bool _wasCollected;
    ChestCollector _collector;

    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        HandleSaveData(saveData);

        Brokers.Default.Receive<SaveDataChangeEvent>()
            .Subscribe(e => HandleSaveData(e.SaveData))
            .AddTo(this);
    }

    void HandleSaveData(SaveData saveData)
    {
        if (saveData != null && saveData.CompletedFlags != null)
        {
            if (saveData.CompletedFlags.Contains(_uid))
            {
                _wasCollected = true;
            }
            else
            {
                _wasCollected = false;
            }
        }

        if (_renderer != null)
            _renderer.sprite = _wasCollected ? _open : _close;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_wasCollected)
            return;

        var controller = collision.GetComponent<ChestCollector>();
        if (controller)
        {
            _collector = controller;
            Brokers.Default.Publish(new EnterPromptEvent());
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (_wasCollected)
            return;

        var controller = collision.GetComponent<ChestCollector>();
        if (controller)
        {
            _collector = null;
            Brokers.Default.Publish(new ExitPromptEvent());
        }
    }

    void Update()
    {
        if (_collector != null && _collector.RequestedCollect)
        {
            GameSaveSystem.CacheGame(_uid);
            Brokers.Audio.Publish(new AudioEvent(_sfx, 0.5f));
            Brokers.Default.Publish(new ItemPickUpEvent(_itemId, _count));
            Brokers.Default.Publish(new ExitPromptEvent());
            _collector = null;
            _wasCollected = true;
        }
    }
}
