using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public EnemyController EnemyController { get; private set; }

    void Start()
    {
        EnemyController = GetComponent<EnemyController>();
    }

    public virtual void DoLogic() { }
}
