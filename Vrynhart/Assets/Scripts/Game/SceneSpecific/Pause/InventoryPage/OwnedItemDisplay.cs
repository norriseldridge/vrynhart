using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class OwnedItemDisplay : MonoBehaviour
{
    static Color SelectedColor = new Color(1, 1, 1, 1);
    static Color UnselectedColor = new Color(1, 1, 1, 0.6f);
    static Color EquippedColor = new Color(1, 1, 0, 1);
    static Color MouseOverColor = new Color(1, 1, 0, 0.6f);

    [SerializeField]
    Image _image;

    [SerializeField]
    Image _border;

    [SerializeField]
    Text _countText;

    string _tab;
    ItemRecord _item;
    public ItemRecord Item => _item;
    bool _equipped = false;
    bool _mouseOver = false;

    ItemRecord _lastSelected;

    void Start()
    {
        Brokers.Default.Receive<InventoryChangeEvent>()
            .Subscribe(OnInventoryChange)
            .AddTo(this);

        Brokers.Default.Receive<PauseItemSelectedEvent>()
            .Subscribe(OnItemSelected)
            .AddTo(this);
    }

    public void SetTab(string tab) => _tab = tab;

    public void PopulateWithItem(ItemRecord item, int ownedCount)
    {
        _item = item;
        _image.sprite = _item.Sprite;
        _countText.text = item.Unique ? "" : ownedCount.ToString();
        _border.color = UnselectedColor;
    }

    public void SetEquipped(bool equipped)
    {
        _equipped = equipped;
        _border.color = _equipped ? EquippedColor : UnselectedColor;
    }

    public void OnClick()
    {
        Brokers.Default.Publish(new PauseItemSelectedEvent(_tab, _item));
    }

    public void OnMouseOver()
    {
        _mouseOver = true;
        UpdateColor();
    }

    public void OnMouseExit()
    {
        _mouseOver = false;
        UpdateColor();
    }

    void OnInventoryChange(InventoryChangeEvent e)
    {
        var owned = e.Inventory.GetCount(_item.Id);
        _countText.text = owned.ToString();
    }

    void OnItemSelected(PauseItemSelectedEvent e)
    {
        if (e.Tab != _tab)
            return;

        _lastSelected = e.Item;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (_lastSelected != null && _lastSelected.Id == _item.Id)
        {
            _border.color = SelectedColor;
        }
        else
        {
            _border.color = _mouseOver ? MouseOverColor : UnselectedColor;
        }
    }
}
