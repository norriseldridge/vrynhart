#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class LevelValidator
{
    [MenuItem("Vrynhart/Level/Validate All")]
    public static void ValidateAll()
    {
        var current = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        var valid = true;
        var levels = Directory.EnumerateFiles(Constants.Editor.LevelsPath).Where(f => !f.Contains("meta") && !f.Contains("Template"));
        foreach (var level in levels)
            valid &= ValidateLevel(level);

        if (!string.IsNullOrEmpty(current))
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(Path.Combine(Constants.Editor.LevelsPath, $"{current}.unity"));

        if (valid)
            Debug.Log("Validation complete! Looks good.");
        else
            Debug.LogError("Level Validation failed.");
    }

    static bool ValidateLevel(string level)
    {
        return ValidateLevelExitsCorrespondWithStartingPoints(level);
    }

    static bool ValidateLevelExitsCorrespondWithStartingPoints(string level)
    {
        var levelPointsMapping = new Dictionary<string, List<string>>();

        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(level);
        var levelExits = Object.FindObjectsOfType<LevelExit>();
        foreach (var exit in levelExits)
        {
            var scene = exit.NextLevel;
            var startingPoint = exit.StartingPoint;

            if (!levelPointsMapping.ContainsKey(scene))
                levelPointsMapping[scene] = new List<string>();

            levelPointsMapping[scene].Add(startingPoint);
        }

        var errorsCount = 0;
        foreach (var mapping in levelPointsMapping)
        {
            var startingPointIds = mapping.Value.Distinct();
            var scene = Path.Combine(Constants.Editor.LevelsPath, $"{mapping.Key}.unity");
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene);
            var startingPoints = Object.FindObjectsOfType<StartingPoint>();
            var names = new List<string>();
            foreach (var sp in startingPoints)
                names.AddRange(sp.Points.Select(p => p.name));

            var invalid = startingPointIds.Where(s => !names.Contains(s));
            errorsCount += invalid.Count();
            foreach (var id in invalid)
                Debug.LogError($"Level `{scene}` does not contain starting point id `{id}`! This id is referenced in level `{level}`!");
        }

        return errorsCount == 0;
    }
}
#endif