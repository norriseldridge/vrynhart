public class HealthChangeEvent
{
    public Health Health { get; private set; }
    public float Change { get; private set; }
    public HealthChangeEvent(Health health, float change)
    {
        Health = health;
        Change = change;
    }
}
