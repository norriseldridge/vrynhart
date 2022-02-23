using UnityEngine;
using UnityEngine.UI;

public class InventoryPage : PausePage
{
    public const string Equipment = "equipment";
    public const string Resources = "resources";
    public const string Clothes = "clothes";
    public const string Keys = "keys";

    [SerializeField]
    Button _equipmentTabButton;

    [SerializeField]
    Button _resourcesTabButton;

    [SerializeField]
    Button _clothesTabButton;

    [SerializeField]
    Button _keysTabButton;

    [SerializeField]
    EquipmentPage _equipmentPage;

    [SerializeField]
    ResourcesPage _resourcesPage;

    [SerializeField]
    ClothesPage _clothesPage;

    [SerializeField]
    KeysPage _keysPage;

    public override void Initialize(PlayerController player)
    {
        _equipmentPage.Initialize(player);
        _resourcesPage.Initialize(player);
        _clothesPage.Initialize(player);
        _keysPage.Initialize(player);

        OnClickTab(Equipment);
    }

    public void OnClickTab(string tab)
    {
        _equipmentTabButton.GetComponent<Image>().color = (tab == Equipment) ? Color.white : Color.gray;
        _equipmentPage.gameObject.SetActive(tab == Equipment);

        _resourcesTabButton.GetComponent<Image>().color = (tab == Resources) ? Color.white : Color.gray;
        _resourcesPage.gameObject.SetActive(tab == Resources);

        _clothesTabButton.GetComponent<Image>().color = (tab == Clothes) ? Color.white : Color.gray;
        _clothesPage.gameObject.SetActive(tab == Clothes);

        _keysTabButton.GetComponent<Image>().color = (tab == Keys) ? Color.white : Color.gray;
        _keysPage.gameObject.SetActive(tab == Keys);
    }
}
