using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class PersistentFlagTileEnabler : MonoBehaviour
{
    [SerializeField]
    List<string> _flagsRequired;

    [SerializeField]
    List<Tile> _tilesToEnabled;

    [SerializeField]
    List<Tile> _tilesToDisabled;

    // Start is called before the first frame update
    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();
        HandlePersistentFlags(saveData.CompletedFlags);

        MessageBroker.Default.Receive<SaveDataChangeEvent>()
            .Subscribe(e => HandlePersistentFlags(e.SaveData.CompletedFlags))
            .AddTo(this);
    }

    void HandlePersistentFlags(List<string> flags)
    {
        var allSet = _flagsRequired.All(f =>
        {
            if (f.StartsWith("!"))
                return !flags.Contains(f);
            return flags.Contains(f);
        });
        if (allSet)
        {
            _tilesToEnabled.ForEach(tile => tile.enabled = true);
            _tilesToDisabled.ForEach(tile => tile.enabled = false);
        }
    }
}
