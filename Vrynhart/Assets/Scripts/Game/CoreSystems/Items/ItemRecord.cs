using UnityEngine;

[CreateAssetMenu(menuName = "Blood/Item")]
public class ItemRecord : ScriptableObject
{
    public string Id;
    public ItemType ItemType;
    public Sprite Sprite;
    public string Name;
    public bool Unique = false;
    public int Cost;

    [TextArea(5, 5)]
    public string ShortDescription;

    [TextArea(5, 10)]
    public string Description;

    // Equippable specific
    [HideInInspector]
    public int UseRange = 1;

    // Clothing specific
    [HideInInspector]
    public RuntimeAnimatorController ClothingAnimator;
}
