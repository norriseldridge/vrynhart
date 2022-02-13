using UnityEngine;
using UniRx;

[ExecuteInEditMode]
public class TurnController : MonoBehaviour
{
    [SerializeField]
    TurnView _view;

    [SerializeField]
    float _turnTime;
    float _time;

    void Start()
    {
        if (_view)
            _view.gameObject.SetActive(true);

        _time = _turnTime;

        MessageBroker.Default.Receive<StopTurnsEvent>()
            .Subscribe(_ => enabled = false)
            .AddTo(this);

        MessageBroker.Default.Receive<TurnProgressionEvent>()
            .Subscribe(_ => _time = _turnTime)
            .AddTo(this);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_view)
                _view.gameObject.SetActive(false);
            return;
        }
#endif
        _time -= Time.deltaTime;

        if (_time <= 0)
            MessageBroker.Default.Publish(new TurnProgressionEvent());

        _view.SetTime(_time, _turnTime);
    }
}
