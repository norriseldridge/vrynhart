#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class Builder
{
    const string AppName = "Vrynhart";

    static string BuildDirectory(BuildTarget target) => Path.Combine(Application.dataPath.Replace("Assets", ""), "Build", target.ToString());
    static string BuildZipDestination(BuildTarget target, string hash) => Path.Combine(Application.dataPath.Replace("Assets", ""), "Build", $"{AppName} ({target}) - {DateTime.Now:MM:dd:yy} ({hash}).zip");
    static string BuildPath(BuildTarget target) => Path.Combine(BuildDirectory(target), AppName);

    static string Hash = null;

    [MenuItem("Vrynhart/Build/Windows", priority = 100)]
    public static void BuildWindows() => Build(BuildTarget.StandaloneWindows64);

    [MenuItem("Vrynhart/Build/Mac", priority = 101)]
    public static void BuildMac() => Build(BuildTarget.StandaloneOSX);

    [MenuItem("Vrynhart/Build/All", priority = 150)]
    public static void BuildAll()
    {
        Hash = $"{DateTime.Now.Ticks}";
        Build(BuildTarget.StandaloneWindows64);
        Build(BuildTarget.StandaloneOSX);
        Hash = null;
    }

    static void Build(BuildTarget target)
    {
        // clean any previous builds
        if (Directory.Exists(BuildDirectory(target)))
            Directory.Delete(BuildDirectory(target), true);

        var options = CreateOptions(target);
        EditorUserBuildSettings.SwitchActiveBuildTarget(options.targetGroup, options.target);
        var result = BuildPipeline.BuildPlayer(options);

        if (result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            PostProcess(target);

            Debug.Log("Build Success!");
        }
        else
        {
            Debug.LogError("Build Failed!");
        }
    }

    static BuildPlayerOptions CreateOptions(BuildTarget target)
    {
        var options = new BuildPlayerOptions();
        options.target = target;
        options.targetGroup = BuildTargetGroup.Standalone;

        options.locationPathName = BuildPath(target);

        var scenes = Directory.EnumerateFiles(Path.Combine(Application.dataPath, "Scenes"), "*unity", SearchOption.AllDirectories)
            .Where(s => !s.Contains("Sandbox") || !s.Contains("Init"));
        var allScenes = new List<string>();
        allScenes.Add("Assets/Scenes/Init.unity");
        allScenes.AddRange(scenes);
        options.scenes = allScenes.ToArray();

        return options;
    }

    static void PostProcess(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows64:
                var path = BuildPath(target);
                if (File.Exists($"{path}.exe"))
                    File.Delete($"{path}.exe");
                File.Move(path, $"{path}.exe");
                break;

            default:
            case BuildTarget.StandaloneOSX:
                break;
        }

        // zip the build for eazy distro
        if (Directory.Exists(BuildDirectory(target)))
        {
            var zipName = BuildZipDestination(target, Hash ?? $"{DateTime.Now.Ticks}");
            ZipFile.CreateFromDirectory(BuildDirectory(target), zipName);
            EditorUtility.RevealInFinder(zipName);
        }
        else
        {
            Debug.Log($"{BuildDirectory(target)} does not exist!");
        }
    }
}
#endif