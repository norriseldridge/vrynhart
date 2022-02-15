using UnityEngine;

public class PlayerChestCollector : ChestCollector
{
    public override bool RequestedCollect => _requestedCollect;
    bool _requestedCollect = false;

    void Update()
    {
        _requestedCollect = Input.GetKeyDown(KeyCode.Space);
    }
}
