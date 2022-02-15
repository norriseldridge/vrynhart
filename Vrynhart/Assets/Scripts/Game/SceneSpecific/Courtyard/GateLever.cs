using UnityEngine;
using UniRx;

public class GateLever : MonoBehaviour
{
    PlayerController _player;

    void Update()
    {
        if (enabled && _player != null && Input.GetKeyDown(KeyCode.Space))
        {
            enabled = false;

            _player = null;
            MessageBroker.Default.Publish(new ExitPromptEvent());
            MessageBroker.Default.Publish(new GateOpenEvent());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
            return;

        var player = collision.GetComponent<PlayerController>();
        if (player)
        {
            _player = player;
            MessageBroker.Default.Publish(new EnterPromptEvent());
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled)
            return;

        var player = collision.GetComponent<PlayerController>();
        if (player)
        {
            _player = null;
            MessageBroker.Default.Publish(new ExitPromptEvent());
        }
    }
}
