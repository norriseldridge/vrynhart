public class PlayerEquippedItemChangeEvent
{
    public ItemRecord Item { get; private set; }
    public PlayerEquippedItemChangeEvent(ItemRecord item)
    {
        Item = item;
    }
}
