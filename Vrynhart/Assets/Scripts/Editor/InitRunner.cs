#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class InitRunner
{
    const string MenuPath = "Vrynhart/InitRunner/Load Init";
    const string InitRunnerSetting = "LoadInit";

    static bool IsEnabled
    {
        get => EditorPrefs.GetBool(InitRunnerSetting, true);
        set => EditorPrefs.SetBool(InitRunnerSetting, value);
    }

    static InitRunner()
    {
        Menu.SetChecked(MenuPath, IsEnabled);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void OnReload()
    {
        Menu.SetChecked(MenuPath, IsEnabled);
    }

    [MenuItem(MenuPath, priority = 0)]
    static void Toggle()
    {
        IsEnabled = !IsEnabled;
        Menu.SetChecked(MenuPath, IsEnabled);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforePlay()
    {
        var message = IsEnabled ?
            "InitRunner is enabled, will start at the Init scene." :
            "InitRunner is NOT enabled, will just launch the current scene.";
        Debug.Log(message);

        if (IsEnabled)
            EditorSceneManager.LoadScene("Init");
    }
}
#endif