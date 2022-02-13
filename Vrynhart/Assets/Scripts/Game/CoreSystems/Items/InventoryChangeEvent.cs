public class InventoryChangeEvent
{
    public Inventory Inventory { get; private set; }

    public InventoryChangeEvent(Inventory inventory)
    {
        Inventory = inventory;
    }
}
