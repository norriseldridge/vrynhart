public class TileMoveCompleteEvent
{
    public TileMover Mover { get; private set; }

    public TileMoveCompleteEvent(TileMover mover)
    {
        Mover = mover;
    }
}
