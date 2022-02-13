using UnityEngine;
using UnityEngine.UI;

public class InventoryPage : PausePage
{
    public const string Equipment = "equipment";
    public const string Clothes = "clothes";
    public const string Keys = "keys";

    [SerializeField]
    Button _equipmentTabButton;

    [SerializeField]
    Button _clothesTabButton;

    [SerializeField]
    Button _keysTabButton;

    [SerializeField]
    EquipmentPage _equipmentPage;

    [SerializeField]
    ClothesPage _clothesPage;

    [SerializeField]
    KeysPage _keysPage;

    public override void Initialize(PlayerController player)
    {
        _equipmentPage.Initialize(player);
        _clothesPage.Initialize(player);
        _keysPage.Initialize(player);

        OnClickTab(Equipment);
    }

    public void OnClickTab(string tab)
    {
        _equipmentTabButton.GetComponent<Image>().color = (tab == Equipment) ? Color.white : Color.gray;
        _equipmentPage.gameObject.SetActive(tab == Equipment);

        _clothesTabButton.GetComponent<Image>().color = (tab == Clothes) ? Color.white : Color.gray;
        _clothesPage.gameObject.SetActive(tab == Clothes);

        _keysTabButton.GetComponent<Image>().color = (tab == Keys) ? Color.white : Color.gray;
        _keysPage.gameObject.SetActive(tab == Keys);
    }
}
