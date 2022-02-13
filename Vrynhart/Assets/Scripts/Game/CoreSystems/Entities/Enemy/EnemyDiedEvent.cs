public class EnemyDiedEvent
{
    public EnemyController EnemyController { get; private set; }
    public EnemyDiedEvent(EnemyController controller)
    {
        EnemyController = controller;
    }
}
