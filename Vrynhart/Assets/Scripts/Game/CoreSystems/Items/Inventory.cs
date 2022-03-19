using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    [Newtonsoft.Json.JsonProperty]
    private Dictionary<string, int> _itemsOwned;

    public Inventory(Dictionary<string, int> items = null)
    {
        _itemsOwned = items ?? new Dictionary<string, int>();
    }

    public Dictionary<string, int> GetOwnedItemIds() => _itemsOwned;

    public List<(ItemRecord item, int count)> GetOwnedItems()
    {
        var items = new List<(ItemRecord, int)>();
        foreach (var pair in _itemsOwned)
        {
            items.Add((ItemsLookup.GetItem(pair.Key), pair.Value));
        }

        return items;
    }

    public void Set(Inventory inventory)
    {
        _itemsOwned = inventory._itemsOwned;

        Brokers.Default.Publish(new InventoryChangeEvent(this));
    }

    public void AddItem(string itemId, int count = 1)
    {
        if (!_itemsOwned.ContainsKey(itemId))
        {
            _itemsOwned[itemId] = count;
        }
        else
        {
            _itemsOwned[itemId] += count;
        }

        Brokers.Default.Publish(new InventoryChangeEvent(this));
    }

    public void RemoveItem(string itemId, int count = 1)
    {
        if (!_itemsOwned.ContainsKey(itemId))
            throw new Exception($"No record found for inventory item: {itemId}");

        if (_itemsOwned[itemId] < count)
            throw new Exception($"You are trying to remove {count}x {itemId} but only own {_itemsOwned[itemId]}!");

        _itemsOwned[itemId] -= count;

        Brokers.Default.Publish(new InventoryChangeEvent(this));
    }

    public int GetCount(string itemId)
    {
        return _itemsOwned.ContainsKey(itemId) ? _itemsOwned[itemId] : 0;
    }
}
