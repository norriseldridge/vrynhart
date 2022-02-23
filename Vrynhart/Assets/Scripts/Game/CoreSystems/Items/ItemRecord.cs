using UnityEngine;

[CreateAssetMenu(menuName = "Vrynhart/Item")]
public class ItemRecord : ScriptableObject
{
    public string Id;
    public ItemType ItemType;
    public Sprite Sprite;
    public string Name;
    public bool Unique = false;
    public int Cost;
    public int PurchaseQuantity = 1;

    [TextArea(5, 5)]
    public string ShortDescription;

    [TextArea(5, 10)]
    public string Description;

    // Equippable specific
    [HideInInspector]
    public int UseRange = 1;

    [HideInInspector]
    public bool ShowUseIndicator = true;

    // Clothing specific
    [HideInInspector]
    public RuntimeAnimatorController ClothingAnimator;
}
