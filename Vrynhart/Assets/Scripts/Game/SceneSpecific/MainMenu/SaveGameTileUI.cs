using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameTileUI : MonoBehaviour
{
    [SerializeField]
    Text _name;

    [SerializeField]
    Text _location;

    [SerializeField]
    Text _date;

    [SerializeField]
    Text _timePlayed;

    Button _button;
    string _saveFile;

    public void Populate(string saveFile, Action<string> onClick)
    {
        _saveFile = saveFile;
        var saveData = DataStorage.Load<SaveData>(Path.Combine(saveFile, Constants.Data.SaveFile));
        _name.text = saveData.Name;
        _location.text = LevelNameLookup.GetDisplayName(saveData.Scene);
        _date.text = saveData.LastPlayed.ToLocalTime().ToString("MM/dd/yy H:mm:ss");

        var played = TimeSpan.FromSeconds(saveData.TotalPlayedTimeSeconds);
        _timePlayed.text = $"{played.Hours}:{played.Minutes:D2}:{played.Seconds:D2}";

        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => onClick.Invoke(_saveFile));
    }

    void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
