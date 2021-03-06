using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RequiresPrompt : MonoBehaviour
{
    public PromptUser PromptUser => _promptUser;
    PromptUser _promptUser;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var user = collision.GetComponent<PromptUser>();
        if (user)
        {
            _promptUser = user;
            Brokers.Default.Publish(new EnterPromptEvent());
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var user = collision.GetComponent<PromptUser>();
        if (user)
        {
            _promptUser = null;
            Brokers.Default.Publish(new ExitPromptEvent());
        }
    }
}
