using UnityEngine;

public class PlayerLevelExiter : LevelExiter
{
    public override bool RequestedExit => _requestedExit;
    bool _requestedExit = false;

    void Update()
    {
        _requestedExit = Input.GetKeyDown(KeyCode.Space);
    }
}
