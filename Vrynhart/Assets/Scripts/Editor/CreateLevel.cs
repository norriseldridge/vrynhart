#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CreateLevel
{
    const string TEMPLATE = "Assets/Scenes/Levels/Template.unity";

    [MenuItem("Vrynhart/Level/Create", priority = 50)]
    public static void Create()
    {
        string file = EditorUtility.SaveFilePanelInProject("Create Level",
            "NewLevel",
            "unity",
            "Create Level",
            "Assets/Scenes/Levels");

        if (file == null)
            return;

        FileUtil.CopyFileOrDirectory(TEMPLATE, file);
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(file);
    }
}
#endif