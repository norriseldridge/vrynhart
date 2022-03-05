using UnityEngine;
using UnityEngine.UI;

public class QuickItemSlot : MonoBehaviour
{
    [SerializeField]
    Image _itemImage;

    [SerializeField]
    Text _countText;

    int _index;

    public void Populate(int index, ItemRecord item, int count)
    {
        _index = index;
        if (item != null)
        {
            _itemImage.sprite = item.Sprite;
            _itemImage.enabled = true;
        }
        else
            _itemImage.enabled = false;
        _countText.text = count > 0 ? count.ToString() : "";
    }

    public void OnClick()
    {
        Brokers.Default.Publish(new QuickSelectSlotClickedEvent(_index));
    }
}
