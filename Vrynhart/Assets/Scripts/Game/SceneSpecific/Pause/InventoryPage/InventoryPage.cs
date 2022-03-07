using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPage : PausePage
{
    public const string Equipment = "equipment";
    public const string Resources = "resources";
    public const string Clothes = "clothes";
    public const string Keys = "keys";

    [SerializeField]
    ControllerButtonListNavigation _buttons;

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

    List<PausePage> _tabPages;

    public override void Initialize(PlayerController player)
    {
        _equipmentPage.Initialize(player);
        _resourcesPage.Initialize(player);
        _clothesPage.Initialize(player);
        _keysPage.Initialize(player);

        _tabPages = new List<PausePage>()
        {
            _equipmentPage,
            _resourcesPage,
            _clothesPage,
            _keysPage
        };
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

    protected override void Update()
    {
        var activePages = _tabPages.Count(p => p.gameObject.activeSelf);
        _buttons.enabled = activePages == 0;
        if (CustomInput.GetKeyDown(CustomInput.Cancel) && activePages == 0)
            gameObject.SetActive(false);
    }
}
