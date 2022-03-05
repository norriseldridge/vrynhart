using System.Threading.Tasks;
using UnityEngine;
using UniRx;

public enum TransitionType
{
    Start,
    End
}

public class TransitionController : MonoBehaviour
{
    public static Task TriggerTransitionAsTask()
    {
        Brokers.Default.Publish(new TransitionEvent(TransitionType.Start));
        return Brokers.Default.Receive<TransitionCompleteEvent>()
            .Where(e => e.Type == TransitionType.Start)
            .Take(1)
            .ToTask();
    }


    bool _open = false;
    TransitionType _type;
    TransitionView _view;

    void Awake()
    {
        DontDestroyOnLoad(this);

        Brokers.Default.Receive<TransitionEvent>()
            .Subscribe(OnTransitionEvent)
            .AddTo(this);

        Brokers.Default.Receive<TransitionCompleteEvent>()
            .Subscribe(OnTransitionCompleteEvent)
            .AddTo(this);
    }

    void Start()
    {
        _view = FindObjectOfType<TransitionView>();
    }

    void OnTransitionEvent(TransitionEvent e)
    {
        _type = e.Type;
        StartTransition();
    }

    void StartTransition()
    {
        if (_type == TransitionType.Start && !_open)
        {
            _open = true;
            _view.FadeToBlack();
        }
        else
            _view.FadeFromBlack();
    }

    void OnTransitionCompleteEvent(TransitionCompleteEvent e)
    {
        if (e.Type == TransitionType.End)
        {
            _open = false;
        }
    }
}
