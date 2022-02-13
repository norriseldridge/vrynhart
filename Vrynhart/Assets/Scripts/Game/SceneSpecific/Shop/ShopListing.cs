using UnityEngine;
using UnityEngine.UI;

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
    Button _buy;

    ItemRecord _item;

    void OnDestroy()
    {
        _buy.onClick.RemoveAllListeners();
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

    public void PopulateWithItem(string item_id)
    {
        _item = ItemsLookup.GetItem(item_id);
        _image.sprite = _item.Sprite;
        _title.text = _item.Name;
        _description.text = _item.ShortDescription;
        _cost.text = _item.Cost.ToString();
    }
}
