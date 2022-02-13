using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveDataEditor : EditorWindow
{
    Vector2 scrollPosition;

    string saveFile;
    SaveData saveData;
    string itemId;
    int count;

    bool completedFlags;
    bool inventory;
    bool clothes;

    [MenuItem("Blood/Save Data Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SaveDataEditor window = GetWindow<SaveDataEditor>(typeof(SaveDataEditor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Save Data", EditorStyles.boldLabel);
        saveFile = EditorGUILayout.TextField("Save File", saveFile);

        if (GUILayout.Button("Load In Save File") && !string.IsNullOrEmpty(saveFile))
        {
            LoadSaveFileIn();
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        if (saveData != null)
        {
            var style = new GUIStyle();
            style.normal.background = new Texture2D(1, 1);
            style.normal.background.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f));
            style.normal.background.Apply();

            completedFlags = EditorGUILayout.Foldout(completedFlags, "Completed Flags");
            if (completedFlags)
            {
                EditorGUILayout.BeginVertical(style);
                HandleCompleteFlags();
                EditorGUILayout.EndVertical();
            }

            inventory = EditorGUILayout.Foldout(inventory, "Inventory");
            if (inventory)
            {
                EditorGUILayout.BeginVertical(style);
                HandleInventory();
                EditorGUILayout.Separator();
                HandleQuickItems();
                EditorGUILayout.EndVertical();
            }

            clothes = EditorGUILayout.Foldout(clothes, "Clothes");
            if (clothes)
            {
                EditorGUILayout.BeginVertical(style);
                HandleClothes();
                EditorGUILayout.EndVertical();
            }

            HandleHealth();

            if (GUILayout.Button("Save Changes"))
            {
                var file = $"{saveFile}/save.json";
                DataStorage.Save(saveData, file);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    void LoadSaveFileIn()
    {
        var file = $"{saveFile}/save.json";
        DataStorage.TryLoad(file, out saveData);
    }

    void HandleCompleteFlags()
    {
        var flagsAsText = string.Join("\n", saveData.CompletedFlags);
        var height = (saveData.CompletedFlags.Count + 1) * 22;
        flagsAsText = EditorGUILayout.TextArea(flagsAsText, GUILayout.Height(height));
        saveData.CompletedFlags = new List<string>(flagsAsText.Split('\n'));
    }

    void HandleQuickItems()
    {
        GUILayout.Label("Quick Items");
        if (saveData.QuickItems == null)
            saveData.QuickItems = new List<string>();

        var quickItemsAsText = string.Join("\n", saveData.QuickItems);
        var height = (saveData.QuickItems.Count + 1) * 22;
        quickItemsAsText = EditorGUILayout.TextArea(quickItemsAsText, GUILayout.Height(height));
        saveData.QuickItems = new List<string>();
        if (!string.IsNullOrEmpty(quickItemsAsText))
            saveData.QuickItems.AddRange(quickItemsAsText.Split('\n'));

        saveData.QuickSelectIndex = EditorGUILayout.IntField("Equipped Index", saveData.QuickSelectIndex);
    }

    void HandleInventory()
    {
        if (saveData.Inventory == null)
            saveData.Inventory = new Inventory();

        var items = saveData.Inventory.GetOwnedItemIds();
        var itemsAsText = "";
        foreach (var pair in items)
            itemsAsText += $"{pair.Key}, {pair.Value}\n";

        GUILayout.Label(itemsAsText);

        EditorGUILayout.BeginHorizontal();
        itemId = EditorGUILayout.TextField("Item Id", itemId);
        count = EditorGUILayout.IntField("Count", count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            saveData.Inventory.AddItem(itemId, count);
        }

        if (GUILayout.Button("Remove"))
        {
            saveData.Inventory.RemoveItem(itemId, count);
        }
        EditorGUILayout.EndHorizontal();
    }

    void HandleClothes()
    {
        var view = saveData.ViewData;
        if (view != null)
        {
            EditorGUILayout.BeginHorizontal();
            view.HatId = EditorGUILayout.TextField("Hat Id", view.HatId);
            view.ClothesId = EditorGUILayout.TextField("Clothes Id", view.ClothesId);
            EditorGUILayout.EndHorizontal();
        }
    }

    void HandleHealth()
    {
        saveData.Health = EditorGUILayout.IntField("Health", saveData.Health);
    }
}
