using UnityEngine;

public class MoveTrigger : MonoBehaviour
{
    [SerializeField]
    Transform _triggerTarget;

    [SerializeField]
    Transform _object;

    [SerializeField]
    Transform _destination;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
            return;

        if (collision.transform == _triggerTarget)
        {
            _object.position = _destination.position;
            enabled = false;
        }
    }
}
