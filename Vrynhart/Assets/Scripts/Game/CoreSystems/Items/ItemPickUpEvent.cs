public class ItemPickUpEvent
{
    public string ItemId { get; set; }
    public int Count { get; set; }
    public ItemPickUpEvent(string itemId, int count)
    {
        ItemId = itemId;
        Count = count;
    }
}
