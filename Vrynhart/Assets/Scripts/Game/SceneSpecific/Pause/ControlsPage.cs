using UnityEngine;

public class ControlsPage : PausePage
{
    [SerializeField]
    GameObject _pc;

    [SerializeField]
    GameObject _controller;

    protected override void Update()
    {
        base.Update();

        var isController = CustomInput.IsController();
        _pc.SetActive(!isController);
        _controller.SetActive(isController);
    }
}
