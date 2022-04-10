using UnityEngine;

public class WerewolfSummonEvent
{
    public Vector3 Position { get; private set; }
    public WerewolfSummonEvent(Vector3 position)
    {
        Position = position;
    }
}
