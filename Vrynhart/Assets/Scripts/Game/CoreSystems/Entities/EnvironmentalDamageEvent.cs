using UnityEngine;

public class EnvironmentalDamageEvent
{
    public Vector3 Position { get; private set; }
    public int Damage { get; private set; }
    public Transform Ignore { get; private set; }

    public EnvironmentalDamageEvent(Vector3 position, int damage, Transform ignore = null)
    {
        Position = position;
        Damage = damage;
        Ignore = ignore;
    }
}
