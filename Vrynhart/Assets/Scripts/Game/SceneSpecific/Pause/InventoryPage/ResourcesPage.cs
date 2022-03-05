using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ResourcesPage : PausePage
{
    [SerializeField]
    OwnedItemDisplay _source;

    [SerializeField]
    GameObject _details;

    [SerializeField]
    Transform _itemsContainer;

    [SerializeField]
    Text _itemName;

    [SerializeField]
    Text _itemDescription;

    ItemRecord _selectedItem;

    public override void Initialize(PlayerController player)
    {
        Brokers.Default.Receive<PauseItemSelectedEvent>()
            .Subscribe(OnItemChanged)
            .AddTo(this);

        var inventory = player.Inventory;

        var ownedItems = inventory.GetOwnedItems();
        var filteredSortedList = ownedItems
            .Where(oi => oi.count > 0 && oi.item.ItemType == ItemType.Currency)
            .OrderBy(oi => oi.item.Name);
        foreach (var ownedItem in filteredSortedList)
        {
            var item = ownedItem.item;
            var count = ownedItem.count;
            var display = Instantiate(_source, _itemsContainer);
            display.SetTab(InventoryPage.Keys);
            display.PopulateWithItem(item, count);
        }

        var first = filteredSortedList.Count() > 0 ? filteredSortedList.First().item : null;
        SelectItem(first);
    }

    void OnItemChanged(PauseItemSelectedEvent e)
    {
        if (!gameObject.activeSelf)
            return;

        if (e.Tab != InventoryPage.Keys)
            return;

        SelectItem(e.Item);
    }

    void SelectItem(ItemRecord item)
    {
        _selectedItem = item;
        if (_selectedItem)
        {
            _details.SetActive(true);
            _itemName.text = _selectedItem.Name;
            _itemDescription.text = _selectedItem.ShortDescription + "\n\n" + _selectedItem.Description;
        }
        else
        {
            _details.SetActive(false);
        }
    }
}
