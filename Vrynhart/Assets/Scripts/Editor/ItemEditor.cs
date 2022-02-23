using UnityEditor;

[CustomEditor(typeof(ItemRecord))]
public class ItemEditor : Editor
{
    SerializedProperty itemType;
    SerializedProperty useRange;
    SerializedProperty showUseIndicator;
    SerializedProperty animatorController;

    void OnEnable()
    {
        itemType = serializedObject.FindProperty("ItemType");
        useRange = serializedObject.FindProperty("UseRange");
        showUseIndicator = serializedObject.FindProperty("ShowUseIndicator");
        animatorController = serializedObject.FindProperty("ClothingAnimator");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        var type = (ItemType)itemType.enumValueIndex;

        if (type == ItemType.Equippable)
        {
            EditorGUILayout.PropertyField(useRange);
            EditorGUILayout.PropertyField(showUseIndicator);
        }

        if (type == ItemType.Clothes || type == ItemType.Hat)
        {
            EditorGUILayout.PropertyField(animatorController);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
