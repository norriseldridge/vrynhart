using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ClothesPage : PausePage
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

    [SerializeField]
    Button _equipButton;

    [SerializeField]
    Text _equipButtonText;

    ItemRecord _currentHat;
    ItemRecord _currentClothes;
    ItemRecord _selectedItem;

    public override void Initialize(PlayerController player)
    {
        Brokers.Default.Receive<PauseItemSelectedEvent>()
            .Subscribe(OnItemChanged)
            .AddTo(this);

        Brokers.Default.Receive<PlayerViewDataEvent>()
            .Subscribe(e => HandleViewData(e.ViewData))
            .AddTo(this);

        var inventory = player.Inventory;
        var ownedItems = inventory.GetOwnedItems();
        var filteredSortedList = ownedItems
            .Where(oi =>
                oi.count > 0 &&
                (oi.item.ItemType == ItemType.Clothes ||
                oi.item.ItemType == ItemType.Hat))
            .OrderBy(oi => oi.item.Name);
        foreach (var ownedItem in filteredSortedList)
        {
            var item = ownedItem.item;
            var count = ownedItem.count;
            var display = Instantiate(_source, _itemsContainer);
            display.SetTab(InventoryPage.Clothes);
            display.PopulateWithItem(item, count);
        }

        var saveData = GameSaveSystem.GetCachedSaveData();
        var viewData = saveData.ViewData;
        HandleViewData(viewData);

        SelectItem(_currentClothes);
    }

    void HandleViewData(PlayerViewData viewData)
    {
        _currentHat = ItemsLookup.GetItem(viewData.HatId);
        _currentClothes = ItemsLookup.GetItem(viewData.ClothesId);

        var itemDisplays = gameObject.GetComponentsInChildren<OwnedItemDisplay>();
        foreach (var itemDisplay in itemDisplays)
        {
            var equipped = (_currentHat != null && _currentHat.Id == itemDisplay.Item.Id) ||
                (_currentClothes != null && _currentClothes.Id == itemDisplay.Item.Id);
            itemDisplay.SetEquipped(equipped);
        }

        SelectItem(_selectedItem);
    }

    public void OnClickWear()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        var viewData = saveData.ViewData;

        if (_selectedItem.ItemType == ItemType.Clothes)
        {
            viewData.ClothesId = _selectedItem.Id;
        }

        if (_selectedItem.ItemType == ItemType.Hat)
        {
            viewData.HatId = _selectedItem.Id;
        }

        GameSaveSystem.CacheGame(viewData);
    }

    void OnItemChanged(PauseItemSelectedEvent e)
    {
        if (!gameObject.activeSelf)
            return;

        if (e.Tab != InventoryPage.Clothes)
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

            // equip button is active only if we don't have this currently equipped
            _equipButton.interactable = !(_selectedItem.Id == _currentClothes.Id || _selectedItem.Id == _currentHat.Id);
            _equipButtonText.text = _equipButton.interactable ? "Equip" : "Equipped";
        }
        else
        {
            _details.SetActive(false);
        }
    }
}
