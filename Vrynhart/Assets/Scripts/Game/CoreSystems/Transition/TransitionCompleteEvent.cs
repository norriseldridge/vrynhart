public class TransitionCompleteEvent
{
    public TransitionType Type { get; private set; }

    public TransitionCompleteEvent(TransitionType type)
    {
        Type = type;
    }
}
