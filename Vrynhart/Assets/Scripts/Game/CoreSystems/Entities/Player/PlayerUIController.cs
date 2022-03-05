using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[System.Serializable]
public class ItemDisplayRecord
{
    public string ItemId;
    public ItemDisplay ItemDisplay;
}

public class PlayerUIController : PostLevelInitialize
{
    const float MaxOil = 120.0f;

    [SerializeField]
    Text _healthText;

    [SerializeField]
    Image _oilFill;

    [SerializeField]
    GameObject _oilFillContainer;

    [SerializeField]
    GameObject _actionPrompt;

    [SerializeField]
    Text _itemPickUpText;

    [Header("Item Display")]
    [SerializeField]
    List<ItemDisplayRecord> _displayedItems;

    [SerializeField]
    Image _equippedItemImage;

    [SerializeField]
    Text _equippedItemCountText;

    ItemRecord _equippedItem;
    Inventory _inventory;

    void Start()
    {
        _actionPrompt.SetActive(false);
        _itemPickUpText.text = "";
        _oilFill.fillAmount = 0;

        Brokers.Default.Receive<HealthChangeEvent>()
            .Subscribe(OnHealthChange)
            .AddTo(this);

        Brokers.Default.Receive<PlayerDiedEvent>()
            .Subscribe(_ => _healthText.text = "0")
            .AddTo(this);

        Brokers.Default.Receive<EnterPromptEvent>()
            .Subscribe(_ => _actionPrompt.SetActive(true))
            .AddTo(this);

        Brokers.Default.Receive<ExitPromptEvent>()
            .Subscribe(_ => _actionPrompt.SetActive(false))
            .AddTo(this);

        Brokers.Default.Receive<ItemPickUpEvent>()
            .Subscribe(OnItemPickedUp)
            .AddTo(this);

        Brokers.Default.Receive<InventoryChangeEvent>()
            .Subscribe(OnInventoryChanged)
            .AddTo(this);

        Brokers.Default.Receive<PlayerEquippedItemChangeEvent>()
            .Subscribe(OnEquippedItemChanged)
            .AddTo(this);
    }

    public override void Initialize()
    {
        var player = FindObjectOfType<PlayerController>();
        _healthText.text = player.Health.CurrentHealth.ToString();
        _healthText.color = player.Health.HealthPercent == 1 ? new Color(1, 0.9f, 0) : Color.white;
        UpdateItemDisplay(player.Inventory);
        UpdateEquippedItem(player.EquippedItem);
    }

    void OnHealthChange(HealthChangeEvent e)
    {
        if (e.Health.gameObject.GetComponent<PlayerController>() != null)
        {
            _healthText.text = e.Health.CurrentHealth.ToString();
            _healthText.color = e.Health.HealthPercent == 1 ? new Color(1, 0.9f, 0) : Color.white;
        }
    }

    void OnItemPickedUp(ItemPickUpEvent e) => StartCoroutine(DisplayItemMessage(e.ItemId, e.Count));

    IEnumerator DisplayItemMessage(string itemId, int count)
    {
        var itemName = ItemsLookup.GetName(itemId);
        _itemPickUpText.text = count > 1 ? $"Picked up {count}x {itemName}s." : $"Picked up {itemName}.";
        yield return new WaitForSeconds(2);
        _itemPickUpText.text = "";
    }

    void OnInventoryChanged(InventoryChangeEvent e) =>
        UpdateItemDisplay(e.Inventory);

    void UpdateItemDisplay(Inventory inventory)
    {
        _inventory = inventory;
        foreach (var record in _displayedItems)
            record.ItemDisplay.SetCount(_inventory.GetCount(record.ItemId));

        if (_equippedItem != null)
        {
            _equippedItemCountText.text = _inventory.GetCount(_equippedItem.Id).ToString();
        }

        HandleLantern();
    }

    void HandleLantern()
    {
        if (_equippedItem != null && _equippedItem.Id == "lantern")
        {
            _oilFillContainer.SetActive(true);
            _oilFill.fillAmount = _inventory.GetCount("oil") / MaxOil;
        }
        else
        {
            _oilFillContainer.SetActive(false);
            _oilFill.fillAmount = 0;
        }
    }

    void OnEquippedItemChanged(PlayerEquippedItemChangeEvent e) =>
        UpdateEquippedItem(e.Item);

    void UpdateEquippedItem(ItemRecord equipped)
    {
        _equippedItem = equipped;
        if (_equippedItem)
        {
            _equippedItemImage.sprite = _equippedItem.Sprite;
            _equippedItemImage.enabled = true;
            _equippedItemCountText.text = _inventory.GetCount(_equippedItem.Id).ToString();
        }
        else
        {
            _equippedItemImage.enabled = false;
            _equippedItemCountText.text = "";
        }

        HandleLantern();
    }
}
