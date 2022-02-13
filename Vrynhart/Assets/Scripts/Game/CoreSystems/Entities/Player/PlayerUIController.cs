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
    [SerializeField]
    Image _healthFill;

    [SerializeField]
    Image _slowHealthFill;

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

        MessageBroker.Default.Receive<HealthChangeEvent>()
            .Subscribe(OnHealthChange)
            .AddTo(this);

        MessageBroker.Default.Receive<PlayerDiedEvent>()
            .Subscribe(_ => StartCoroutine(TweenFill(0)))
            .AddTo(this);

        MessageBroker.Default.Receive<EnterPromptEvent>()
            .Subscribe(_ => _actionPrompt.SetActive(true))
            .AddTo(this);

        MessageBroker.Default.Receive<ExitPromptEvent>()
            .Subscribe(_ => _actionPrompt.SetActive(false))
            .AddTo(this);

        MessageBroker.Default.Receive<ItemPickUpEvent>()
            .Subscribe(OnItemPickedUp)
            .AddTo(this);

        MessageBroker.Default.Receive<InventoryChangeEvent>()
            .Subscribe(OnInventoryChanged)
            .AddTo(this);

        MessageBroker.Default.Receive<PlayerEquippedItemChangeEvent>()
            .Subscribe(OnEquippedItemChanged)
            .AddTo(this);
    }

    public override void Initialize()
    {
        var player = FindObjectOfType<PlayerController>();
        _healthFill.fillAmount = _slowHealthFill.fillAmount = player.Health.HealthPercent;
        UpdateItemDisplay(player.Inventory);
        UpdateEquippedItem(player.EquippedItem);
    }

    void OnHealthChange(HealthChangeEvent e)
    {
        if (e.Health.gameObject.GetComponent<PlayerController>() != null)
        {
            if (e.Change == 0)
                _healthFill.fillAmount = _slowHealthFill.fillAmount = e.Health.HealthPercent;
            else
                StartCoroutine(TweenFill(e.Health.HealthPercent));
        }
    }

    IEnumerator TweenFill(float target)
    {
        IEnumerator Tween(Image img, int steps)
        {
            var diff = Mathf.Abs(img.fillAmount - target);

            var delta = diff / steps;
            for (int i = 0; i < steps; ++i)
            {
                img.fillAmount = Mathf.MoveTowards(img.fillAmount, target, delta);
                yield return null;
            }
        }

        yield return Tween(_healthFill, 15);
        yield return Tween(_slowHealthFill, 30);
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
    }
}
