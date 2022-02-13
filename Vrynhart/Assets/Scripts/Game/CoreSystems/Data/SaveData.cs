using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string Name;
    public string Scene;
    public int X, Y;
    public Inventory Inventory;
    public List<string> QuickItems;
    public int QuickSelectIndex;
    public int Health;
    public PlayerViewData ViewData;
    public List<string> CompletedFlags;
}
