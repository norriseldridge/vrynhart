using UnityEngine;
using UniRx;

public class Spikes : Tile
{
    [Header("Spike")]
    [SerializeField]
    AudioClip _warningSound;

    [SerializeField]
    float _warningVolume;

    [SerializeField]
    AudioClip _fireSound;

    [SerializeField]
    float _fireVolume;

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
        else if (_currentDelay == _turnDelay - 1)
        {
            MessageBroker.Default.Publish(new AudioEvent(_warningSound,
                _active ? _warningVolume : _warningVolume * 0.6f,
                position: transform.position));
        }

        if (_active)
            MessageBroker.Default.Publish(new EnvironmentalDamageEvent(transform.position, _damage));
    }

    void UpdateVisuals()
    {
        if (_active)
            MessageBroker.Default.Publish(new AudioEvent(_fireSound, _fireVolume, position: transform.position));

        _animator.Play(_active ? _activeAnimation : _inactiveAnimation);
    }
}
