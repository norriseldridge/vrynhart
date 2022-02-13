using System.Collections.Generic;
using UnityEngine;

public class ItemsLookup : MonoBehaviour
{
    static ItemsLookup Instance { get; set; }

    [SerializeField]
    List<ItemRecord> _itemNames;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        Instance = this;
    }

    public static string GetName(string id) => GetItem(id).Name;

    public static int GetCost(string id) => GetItem(id).Cost;
    
    public static ItemRecord GetItem(string id)
    {
        var item = Instance._itemNames.Find(i => i.Id == id);
        if (item != null)
            return item;

        throw new System.Exception($"No item was found for id: {id}");
    }
}
