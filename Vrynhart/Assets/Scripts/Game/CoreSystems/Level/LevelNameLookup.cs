using System.Collections.Generic;

public static class LevelNameLookup
{
    const string Vrynhart = "Vrynhart";
    static Dictionary<string, string> _names = new Dictionary<string, string>()
    {
        { "StartingTown", $"{Vrynhart} Village" },
        { "Vh_Woods", $"{Vrynhart} Woods" },
        { "Sewers", $"{Vrynhart} Sewers" },
        { "Vh_Courtyard", $"{Vrynhart} Courtyard" },
        { "Vh_Tower", $"{Vrynhart} Watchtower" },
        { "Vh_MainHall", $"{Vrynhart} Grand Hall" },
        { "Vh_MainHall2", $"{Vrynhart}'s North Hall" },
        { "Vh_Cellar", $"{Vrynhart} Cellars" },
        { "Dungeon", $"{Vrynhart} Dungeon" }
    };

    public static string GetDisplayName(string sceneName)
    {
        if (_names.TryGetValue(sceneName, out var name))
            return name;

        UnityEngine.Debug.LogError($"{sceneName} not found in LevelNameLookup!");
        return sceneName;
    }
}
