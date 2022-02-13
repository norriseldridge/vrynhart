using UnityEngine;

public class EnemyDestinationPickedEvent
{
    public EnemyController EnemyController { get; private set; }
    public Vector3 Target { get; private set; }
    public EnemyDestinationPickedEvent(EnemyController controller, Vector3 target)
    {
        EnemyController = controller;
        Target = target;
    }
}
