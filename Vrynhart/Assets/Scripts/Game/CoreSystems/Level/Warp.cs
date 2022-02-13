using System.Collections;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Collider2D))]
public class Warp : MonoBehaviour
{
    [SerializeField]
    GameObject _destination;

    [SerializeField]
    bool _transition;

    async void OnTriggerEnter2D(Collider2D collision)
    {
        var user = collision.GetComponent<WarpUser>();
        if (user == null)
            return;

        if (_transition)
            await TransitionController.TriggerTransitionAsTask();

        user.SetPosition(_destination.transform.position);

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
