using UnityEngine;

public class PlayerInputEvent
{
    public Vector3 NextPosition { get; private set; }
    public PlayerInputEvent(Vector3 nextPosition)
    {
        NextPosition = nextPosition;
    }
}
