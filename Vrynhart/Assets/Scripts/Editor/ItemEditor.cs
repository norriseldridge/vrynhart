using UnityEditor;

[CustomEditor(typeof(ItemRecord))]
public class ItemEditor : Editor
{
    SerializedProperty itemType;
    SerializedProperty useRange;
    SerializedProperty showUseIndicator;
    SerializedProperty combatSfx;
    SerializedProperty overrideCountId;
    SerializedProperty animatorController;

    void OnEnable()
    {
        itemType = serializedObject.FindProperty("ItemType");
        useRange = serializedObject.FindProperty("UseRange");
        showUseIndicator = serializedObject.FindProperty("ShowUseIndicator");
        combatSfx = serializedObject.FindProperty("CombatSfx");
        overrideCountId = serializedObject.FindProperty("OverrideCountId");
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
            EditorGUILayout.PropertyField(overrideCountId);
            var color = UnityEngine.GUI.contentColor;
            UnityEngine.GUI.contentColor = new UnityEngine.Color(0.7f, 0.7f, 0.7f);
            EditorGUILayout.LabelField("OverrideCountId is used to display the quantity of a different item when equipped. For example, the 'gun' can show 'bullet' count instead.", EditorStyles.wordWrappedLabel);
            UnityEngine.GUI.contentColor = color;
        }

        if (type == ItemType.Currency)
        {
            EditorGUILayout.PropertyField(combatSfx);
        }

        if (type == ItemType.Clothes || type == ItemType.Hat)
        {
            EditorGUILayout.PropertyField(animatorController);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
