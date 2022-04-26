using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public static class GameSaveSystem
{
    static string _currentSaveFile;
    static SaveData _cache;
    static bool _saved = false;

    public static bool IsSaved() => _saved;

    public static string GetCurrentSaveFile() =>
        Path.Combine(_currentSaveFile, Constants.Data.SaveFile);

    public static void CreateSaveFile(string name)
    {
        _currentSaveFile = name;
        _cache = new SaveData()
        {
            Name = name,
            Inventory = new Inventory(new Dictionary<string, int>()
            {
                { "coin", 15 },
                { "hunter_set", 1 },
                { "hunter_hat", 1 },
                { "lantern", 1 }
            }),
            QuickItems = new List<string>() { "lantern" },
            QuickSelectIndex = 0,
            ViewData = new PlayerViewData()
            {
                HatId = "hunter_hat",
                ClothesId = "hunter_set"
            },
            Health = 3,
            X = 0,
            Y = 0,
            Scene = Constants.Game.StartingScene,
            CompletedFlags = new List<string>(),
            TotalPlayedTimeSeconds = 0,
            LastPlayed = DateTime.UtcNow
        };

        var path = Path.Combine(DataStorage.GetDataPath(), _currentSaveFile);
        Directory.CreateDirectory(path);
        DataStorage.Save(_cache, GetCurrentSaveFile());
        _saved = true;

        Brokers.Default.Publish(new SaveDataChangeEvent(_cache));
    }

    public static bool DoSaveFilesExist() => GetSaveFiles().Length > 0;

    public static string[] GetSaveFiles() => Directory.GetDirectories(DataStorage.GetDataPath())
        .OrderByDescending(d => new FileInfo(Path.Combine(d, Constants.Data.SaveFile)).LastWriteTime)
        .ToArray();

    public static void LoadLastPlayedGame()
    {
        var directories = GetSaveFiles();
        if (directories.Length > 0)
        {
            var lastPlayed = directories.First();
            LoadGame(lastPlayed);
        }
    }

    public static void SaveGame(PlayerController player, string currentScene, Vector3 position)
    {
        _cache.Scene = currentScene;
        _cache.X = (int)position.x;
        _cache.Y = (int)position.y;
        _cache.Inventory = player.Inventory;
        _cache.QuickSelectIndex = player.QuickSelectIndex;
        _cache.QuickItems = player.QuickItems;
        _cache.Health = player.Health.CurrentHealth;

        // update the total played time
        var last = _cache.LastPlayed;
        var now = DateTime.UtcNow;
        var diff = now.Subtract(last).TotalSeconds;
        _cache.TotalPlayedTimeSeconds += diff;
        _cache.LastPlayed = now;

        DataStorage.Save(_cache, GetCurrentSaveFile());
        _saved = true;

        Brokers.Default.Publish(new SaveDataChangeEvent(_cache));
    }

    public static void LoadGame(string file = null)
    {
        if (file != null)
            _currentSaveFile = file;

        if (!DataStorage.TryLoad(GetCurrentSaveFile(), out _cache))
            _cache = new SaveData(); // we try to load but fail, just create a new savedata

        _cache.LastPlayed = DateTime.UtcNow;

        _saved = true; // a freshly loaded game is at the last save, thus "saved"
        Brokers.Default.Publish(new SaveDataChangeEvent(_cache));

        SceneManager.LoadSceneAsync(_cache.Scene);
    }

    public static SaveData GetCachedSaveData() => _cache;

    public static void CacheGame(string currentScene, Vector3 position) // on entering a scene
    {
        _saved = false;
        _cache.Scene = currentScene;
        _cache.X = (int)position.x;
        _cache.Y = (int)position.y;

        Brokers.Default.Publish(new SaveDataChangeEvent(_cache));
    }

    public static void CacheGame(PlayerController player) // on exiting a scene
    {
        _saved = false;
        _cache.Inventory = player.Inventory;
        _cache.QuickSelectIndex = player.QuickSelectIndex;
        _cache.QuickItems = player.QuickItems;
        _cache.Health = player.Health.CurrentHealth;

        Brokers.Default.Publish(new SaveDataChangeEvent(_cache));
    }

    public static void CacheGame(string flag) // on completing a permanent action
    {
        _saved = false;

        if (!_cache.CompletedFlags.Contains(flag))
            _cache.CompletedFlags.Add(flag);

        Brokers.Default.Publish(new SaveDataChangeEvent(_cache));
    }

    public static void CacheGame(PlayerViewData viewData)
    {
        _saved = false;
        _cache.ViewData = viewData;
        Brokers.Default.Publish(new PlayerViewDataEvent(_cache.ViewData));
        Brokers.Default.Publish(new SaveDataChangeEvent(_cache));
    }
}
