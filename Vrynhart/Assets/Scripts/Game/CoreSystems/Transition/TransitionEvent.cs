public class TransitionEvent
{
    public TransitionType Type { get; private set; }

    public TransitionEvent(TransitionType type)
    {
        Type = type;
    }
}
