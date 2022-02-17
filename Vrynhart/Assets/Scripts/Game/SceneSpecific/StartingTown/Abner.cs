using UnityEngine;

public class Abner : MonoBehaviour
{
    [SerializeField]
    string _uid;

    [SerializeField]
    GameObject _abnerStart;

    [SerializeField]
    Tile _startTile;

    [SerializeField]
    GameObject _abnerAfterTalk;

    [SerializeField]
    Tile _afterTalkTile;

    void Start()
    {
        var saveData = GameSaveSystem.GetCachedSaveData();

        var talkedTo = saveData.CompletedFlags.Contains(_uid);
        _abnerStart.SetActive(!talkedTo);
        _startTile.IsFloor = talkedTo;
        _abnerAfterTalk.SetActive(talkedTo);
        _afterTalkTile.IsFloor = !talkedTo;
    }
}
