using UnityEngine;
using UniRx;

[RequireComponent(typeof(CircleCollider2D))]
public class Hazard : MonoBehaviour
{
    [SerializeField]
    int _damage;

    void Start()
    {
        Brokers.Default.Receive<TurnProgressionEvent>()
            .Subscribe(OnTurn)
            .AddTo(this);
    }

    void OnTurn(TurnProgressionEvent e)
    {
        Brokers.Default.Publish(new EnvironmentalDamageEvent(transform.position, _damage));
    }
}
