using UnityEngine;

public class TileMoveEvent
{
    public TileMover Mover { get; private set; }
    public Vector2 Direction { get; private set; }

    public TileMoveEvent(TileMover mover, Vector2 direction)
    {
        Mover = mover;
        Direction = direction;
    }
}