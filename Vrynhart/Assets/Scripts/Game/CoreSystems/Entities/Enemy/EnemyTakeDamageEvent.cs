public class EnemyTakeDamageEvent
{
    public EnemyController EnemyController { get; private set; }
    public int Damage { get; private set; }

    public EnemyTakeDamageEvent(EnemyController controller, int damage)
    {
        EnemyController = controller;
        Damage = damage;
    }
}
