using UnityEngine;
using UniRx;

[RequireComponent(typeof(CircleCollider2D))]
public class Hazard : MonoBehaviour
{
    [SerializeField]
    int _damage;

    void Start()
    {
        MessageBroker.Default.Receive<TurnProgressionEvent>()
            .Subscribe(OnTurn)
            .AddTo(this);
    }

    void OnTurn(TurnProgressionEvent e)
    {
        MessageBroker.Default.Publish(new EnvironmentalDamageEvent(transform.position, _damage));
    }
}
