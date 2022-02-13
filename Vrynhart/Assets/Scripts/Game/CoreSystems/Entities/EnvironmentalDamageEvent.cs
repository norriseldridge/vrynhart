using UnityEngine;

public class EnvironmentalDamageEvent
{
    public Vector3 Position { get; private set; }
    public int Damage { get; private set; }

    public EnvironmentalDamageEvent(Vector3 position, int damage)
    {
        Position = position;
        Damage = damage;
    }
}
