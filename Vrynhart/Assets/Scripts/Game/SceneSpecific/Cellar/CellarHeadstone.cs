using UnityEngine;
using UniRx;

public class CellarHeadstone : MonoBehaviour
{
    [SerializeField]
    EnemyController _enemyController;

    [SerializeField]
    GameObject _headstone;

    public bool Destroyed { get; private set; } = false;

    void Start()
    {
        Brokers.Default.Receive<EnemyDiedEvent>()
            .Subscribe(e => {
                if (e.EnemyController == _enemyController)
                {
                    _headstone.SetActive(false);
                    Destroyed = true;
                }
            })
            .AddTo(this);
    }

    public void MarkDestroyed()
    {
        _headstone.SetActive(false);
        Destroyed = true;
        if (_enemyController != null && _enemyController.gameObject != null)
            _enemyController.enabled = false;
    }
}
