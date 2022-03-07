using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;

public class EquipmentPage : PausePage
{
    [SerializeField]
    OwnedItemDisplay _source;

    [SerializeField]
    ControllerButtonListNavigation _buttons;

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
            .Where(oi => oi.item.ItemType == ItemType.Equippable)
            .OrderBy(oi => oi.item.Unique)
            .OrderBy(oi => oi.item.Name);

        var displayList = new List<Button>();
        foreach (var ownedItem in filteredSortedList)
        {
            var item = ownedItem.item;
            var count = ownedItem.count;
            var display = Instantiate(_source, _itemsContainer);
            display.SetTab(InventoryPage.Equipment);
            display.PopulateWithItem(item, count);
            displayList.Add(display.GetComponent<Button>());
        }
        _buttons.SetOverride(displayList);

        var first = filteredSortedList.Count() > 0 ? filteredSortedList.First().item : null;
        SelectItem(first);
    }

    void OnItemChanged(PauseItemSelectedEvent e)
    {
        if (!gameObject.activeSelf)
            return;

        if (e.Tab != InventoryPage.Equipment)
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

    protected override void Update()
    {
        base.Update();

        var y = CustomInput.GetAxis("Mouse Y");
        var scroll = _itemDescription.GetComponentInParent<ScrollRect>();
        if (scroll && scroll.content)
            scroll.content.localPosition += Vector3.up * y;
    }
}
