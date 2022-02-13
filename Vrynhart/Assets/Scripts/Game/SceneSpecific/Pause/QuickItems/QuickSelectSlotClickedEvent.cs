public class QuickSelectSlotClickedEvent
{
    public int Index { get; private set; }
    public QuickSelectSlotClickedEvent(int index)
    {
        Index = index;
    }
}
