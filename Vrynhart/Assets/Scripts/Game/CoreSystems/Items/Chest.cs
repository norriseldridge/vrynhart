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

    bool _wasCollected;

    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        HandleSaveData(saveData);

        MessageBroker.Default.Receive<SaveDataChangeEvent>()
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

        _renderer.sprite = _wasCollected ? _open : _close;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_wasCollected)
            return;

        var player = collision.GetComponent<PlayerController>();
        if (player)
        {
            GameSaveSystem.CacheGame(_uid);
            MessageBroker.Default.Publish(new ItemPickUpEvent(_itemId, _count));
            _wasCollected = true;
        }
    }
}
