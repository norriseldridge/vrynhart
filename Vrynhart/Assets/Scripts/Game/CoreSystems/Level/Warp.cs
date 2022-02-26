using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public class Warp : RequiresPrompt
{
    [SerializeField]
    GameObject _destination;

    [SerializeField]
    bool _transition;

    [SerializeField]
    bool _requiresPrompt;

    [SerializeField]
    AudioClip _sfx;

    [SerializeField]
    float _volume;

    WarpUser _user;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        var user = collision.GetComponent<WarpUser>();
        if (user == null)
            return;

        _user = user;

        if (_requiresPrompt)
            base.OnTriggerEnter2D(collision);
        else
            DoWarp();
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (_requiresPrompt)
            base.OnTriggerExit2D(collision);

        var user = collision.GetComponent<WarpUser>();
        if (user == null)
            return;

        _user = null;
    }

    void Update()
    {
        if (_user != null && PromptUser != null && PromptUser.AcceptedPrompt)
            DoWarp();
    }

    async void DoWarp()
    {
        if (_sfx)
            MessageBroker.Default.Publish(new AudioEvent(_sfx, _volume));

        if (_transition)
            await TransitionController.TriggerTransitionAsTask();

        if (_user != null)
        {
            _user.SetPosition(_destination.transform.position);
            _user = null;
        }

        if (_transition)
            MessageBroker.Default.Publish(new TransitionEvent(TransitionType.End));
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // highlight warp location
        if (_destination != null)
        {
            Gizmos.color = new Color(1, 1, 0.5f, 0.35f);
            Gizmos.DrawCube(_destination.transform.position, Vector3.one);
        }
    }
#endif
}
