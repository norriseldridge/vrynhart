using System.Collections.Generic;

public static class Constants 
{
    public static class Data
    {
        public const string SaveFile = "save.json";
    }

    public static class Game
    {
        public const string MainMenuScene = "MainMenu";
        public const string StartingScene = "StartingTown";
        public const string PauseScene = "Pause";
        public const string ShopScene = "Shop";
        public const string BossUIScene = "BossUI";
        public const string OutroScene = "Outro";

        public static readonly Dictionary<string, int> TargetItemQuantities = new Dictionary<string, int>()
        {
            { "holy_water", 2 },
            { "coin", 20 },
            { "wood_stake", 3 },
            { "rusty_dagger", 11 },
            { "oil", 80 }
        };
    }

    public static class Prefs
    {
        public const string MusicVolume = "MusicVolume";
        public const string SFXVolume = "SFXVolume";
    }

    public static class Editor
    {
        public const string LevelsPath = "Assets/Scenes/Levels/";
    }
}
