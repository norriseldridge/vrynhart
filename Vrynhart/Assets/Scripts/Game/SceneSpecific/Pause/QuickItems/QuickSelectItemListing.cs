using UnityEngine;
using UnityEngine.UI;

public class QuickSelectItemListing : MonoBehaviour
{
    [SerializeField]
    Image _itemImage;

    [SerializeField]
    Text _nameText;

    [SerializeField]
    Text _shortDescriptionText;

    [SerializeField]
    Text _countText;

    public ItemRecord Item { get; private set; }

    public void Populate(ItemRecord item, int count)
    {
        Item = item;
        _itemImage.sprite = item.Sprite;
        _nameText.text = item.Name;
        _shortDescriptionText.text = item.ShortDescription;
        _countText.text = count.ToString();
    }

    public void OnClick()
    {
        Brokers.Default.Publish(new QuickSelectItemListingClickEvent(this));
    }
}
