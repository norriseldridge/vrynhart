using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.EventSystems;

public class ShopListing : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public delegate bool AttemptPurchase(ItemRecord item);

    [SerializeField]
    Image _image;

    [SerializeField]
    Text _title;

    [SerializeField]
    Text _description;

    [SerializeField]
    Text _cost;

    [SerializeField]
    Text _quantity;

    [SerializeField]
    Text _ownedQuantity;

    [SerializeField]
    Button _buy;

    ItemRecord _item;
    PlayerController _player;
    bool _selected = false;

    void OnDestroy()
    {
        _buy.onClick.RemoveAllListeners();
    }

    public void Initialize(PlayerController player, string itemId)
    {
        _player = player;
        Brokers.Default.Receive<InventoryChangeEvent>()
            .Subscribe(e =>
            {
                _ownedQuantity.text = $"Owned: {_player.Inventory.GetCount(_item.Id)}";
            })
            .AddTo(this);
        PopulateWithItem(itemId);
    }

    public void AddOnBuyCallback(AttemptPurchase onClickBuy)
    {
        _buy.onClick.AddListener(() => {
            if (onClickBuy.Invoke(_item))
            {
                if (_item.Unique)
                {
                    Destroy(gameObject);
                }
            }
        });
    }

    void PopulateWithItem(string itemId)
    {
        _item = ItemsLookup.GetItem(itemId);
        _image.sprite = _item.Sprite;
        _title.text = _item.Name;
        _description.text = _item.ShortDescription;
        _cost.text = _item.Cost.ToString();
        _quantity.text = $"x{_item.PurchaseQuantity}";
        _ownedQuantity.text = $"Owned: {_player.Inventory.GetCount(_item.Id)}";
    }

    public void OnSelect(BaseEventData eventData) => _selected = true;

    public void OnDeselect(BaseEventData eventData) => _selected = false;

    void Update()
    {
        if (CustomInput.IsController())
        {
            if (CustomInput.GetKeyDown(CustomInput.Accept) && _selected)
                _buy.onClick.Invoke();
        }
    }
}
