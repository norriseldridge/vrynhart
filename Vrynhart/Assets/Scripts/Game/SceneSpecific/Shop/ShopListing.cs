using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ShopListing : MonoBehaviour
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

    void OnDestroy()
    {
        _buy.onClick.RemoveAllListeners();
    }

    public void Initialize(PlayerController player, string itemId)
    {
        _player = player;
        MessageBroker.Default.Receive<InventoryChangeEvent>()
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
}
