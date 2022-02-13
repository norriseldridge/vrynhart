public class PauseItemSelectedEvent
{
    public string Tab { get; private set; }
    public ItemRecord Item { get; private set; }
    public PauseItemSelectedEvent(string tab, ItemRecord item)
    {
        Tab = tab;
        Item = item;
    }
}
