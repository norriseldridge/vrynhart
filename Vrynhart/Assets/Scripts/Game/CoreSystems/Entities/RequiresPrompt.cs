using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public class RequiresPrompt : MonoBehaviour
{
    public PromptUser PromptUser => _user;
    PromptUser _user;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var user = collision.GetComponent<PromptUser>();
        if (user)
        {
            _user = user;
            MessageBroker.Default.Publish(new EnterPromptEvent());
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var user = collision.GetComponent<PromptUser>();
        if (user)
        {
            _user = null;
            MessageBroker.Default.Publish(new ExitPromptEvent());
        }
    }
}
