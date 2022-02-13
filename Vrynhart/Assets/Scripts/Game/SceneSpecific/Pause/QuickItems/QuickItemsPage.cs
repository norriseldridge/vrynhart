using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class QuickItemsPage : PausePage
{
    [SerializeField]
    List<QuickItemSlot> _slots;

    [SerializeField]
    QuickSelectItemListing _source;

    [SerializeField]
    GameObject _equipmentDisplay;

    [SerializeField]
    Transform _equipmentContainer;

    PlayerController _player;
    int _selectedSlotIndex = -1;
    List<QuickSelectItemListing> _equipmentListings = new List<QuickSelectItemListing>();

    public override void Initialize(PlayerController player)
    {
        _player = player;
        var quickItems = _player.QuickItems;

        // populate the current quick select items
        UpdateQuickSelectSlots();

        // populate the items listings
        var equipment = _player.Inventory.GetOwnedItems()
            .Where(oi => oi.item.ItemType == ItemType.Equippable);
        foreach (var ownedItem in equipment)
        {
            var listing = Instantiate(_source, _equipmentContainer);
            listing.Populate(ownedItem.item, ownedItem.count);
            _equipmentListings.Add(listing);
        }

        // don't show this equipment
        _equipmentDisplay.SetActive(false);

        MessageBroker.Default.Receive<QuickSelectSlotClickedEvent>()
            .Subscribe(OnQuickSelectSlotClicked)
            .AddTo(this);

        MessageBroker.Default.Receive<QuickSelectItemListingClickEvent>()
            .Subscribe(OnQuickSelectItemListingClickEvent)
            .AddTo(this);
    }

    string GetQuickSelectItemId(int index)
    {
        if (_player.QuickItems.Count > index)
        {
            return _player.QuickItems[index];
        }

        return "";
    }

    void OnQuickSelectSlotClicked(QuickSelectSlotClickedEvent e)
    {
        _selectedSlotIndex = e.Index;

        // filter the currently displayed equipment
        _equipmentListings.ForEach(l => l.gameObject.SetActive(!_player.QuickItems.Contains(l.Item.Id)));

        _equipmentDisplay.SetActive(true);
    }

    void OnQuickSelectItemListingClickEvent(QuickSelectItemListingClickEvent e)
    {
        // set this item in the quick select
        var quickItems = _player.QuickItems;
        while (quickItems.Count <= _selectedSlotIndex)
            quickItems.Add("");
        quickItems[_selectedSlotIndex] = e.QuickSelectItemListing.Item.Id;

        // save this change
        var saveData = GameSaveSystem.GetCachedSaveData();
        saveData.QuickItems = quickItems;
        GameSaveSystem.CacheGame(_player);
        _player.QuickItems = quickItems;

        // hide the display
        UpdateQuickSelectSlots();
        _equipmentDisplay.SetActive(false);

        // if we changed the item we currently have selected / equipped
        if (_selectedSlotIndex == _player.QuickSelectIndex)
        {
            var item = ItemsLookup.GetItem(quickItems[_selectedSlotIndex]);
            MessageBroker.Default.Publish(new PlayerEquippedItemChangeEvent(item));
        }
    }

    void UpdateQuickSelectSlots()
    {
        var inventory = _player.Inventory;
        for (var i = 0; i < _slots.Count; ++i)
        {
            var slot = _slots[i];
            var id = GetQuickSelectItemId(i);

            if (!string.IsNullOrEmpty(id))
                slot.Populate(i, ItemsLookup.GetItem(id), inventory.GetCount(id));
            else
                slot.Populate(i, null, 0);
        }
    }
}
