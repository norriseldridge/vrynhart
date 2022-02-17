using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class CameraTool : EditorWindow
{
    [MenuItem("Vrynhart/Level/Camera Tool", priority = 51)]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CameraTool window = GetWindow<CameraTool>(typeof(CameraTool));
        window.Show();
    }

    float cameraSize;

    void OnGUI()
    {
        cameraSize = EditorGUILayout.FloatField("Camera Size", cameraSize);

        if (GUILayout.Button("Update Cameras"))
        {
            var current = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            var levels = Directory.EnumerateFiles(Constants.Editor.LevelsPath).Where(f => !f.Contains("meta") && !f.Contains("Template"));
            foreach (var level in levels)
            {
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(level);
                var camera = FindObjectOfType<Camera>();
                camera.orthographicSize = cameraSize;
                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            }

            if (!string.IsNullOrEmpty(current))
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(Path.Combine(Constants.Editor.LevelsPath, $"{current}.unity"));
        }
    }
}
