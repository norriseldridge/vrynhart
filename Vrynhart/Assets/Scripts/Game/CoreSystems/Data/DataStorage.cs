using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class DataStorage
{
    static bool _dataPathExists = false;
    
    public static string GetDataPath()
    {
        var path = Path.Combine(Application.dataPath, "_data");
        if (!_dataPathExists)
        {
            Directory.CreateDirectory(path);
            _dataPathExists = true;
        }

        return path;
    }

    public static void Save<T>(T obj, string name)
    {
        var json = JsonConvert.SerializeObject(obj,
            Formatting.None,
            new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
        var path = Path.Combine(GetDataPath(), name);
        File.WriteAllText(path, json);
    }

    public static T Load<T>(string name)
    {
        var path = Path.Combine(GetDataPath(), name);

        if (!File.Exists(path))
            throw new FileNotFoundException($"There is no file found at path: {path}!");

        var json = File.ReadAllText(path);
        var obj = JsonConvert.DeserializeObject<T>(json);
        return obj;
    }

    public static bool TryLoad<T>(string name, out T obj)
    {
        try
        {
            obj = Load<T>(name);
            return true;
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                Debug.LogError(e);
#else
            Debug.LogError(e);
#endif
            obj = default;
            return false;
        }
    }
}
