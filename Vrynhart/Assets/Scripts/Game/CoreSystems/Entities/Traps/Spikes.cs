using UnityEngine;
using UniRx;

public class Spikes : Tile
{
    [Header("Spike")]
    [SerializeField]
    int _turnDelay;

    [SerializeField]
    int _currentDelay = 0;

    [SerializeField]
    int _damage;

    [SerializeField]
    bool _active;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    string _activeAnimation;

    [SerializeField]
    string _inactiveAnimation;

    void Start()
    {
        MessageBroker.Default.Receive<TurnProgressionEvent>()
            .Subscribe(_ => TickDelay())
            .AddTo(this);
        UpdateVisuals();
    }

    void TickDelay()
    {
        _currentDelay++;
        if (_currentDelay >= _turnDelay)
        {
            _currentDelay = 0;
            _active = !_active;
            UpdateVisuals();
        }

        if (_active)
            MessageBroker.Default.Publish(new EnvironmentalDamageEvent(transform.position, _damage));
    }

    void UpdateVisuals()
    {
        _animator.Play(_active ? _activeAnimation : _inactiveAnimation);
    }
}
