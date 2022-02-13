using UnityEngine;
using UniRx;

public class MoveIndicator : PostLevelInitialize
{
    [SerializeField]
    float _scaleAmount;

    [SerializeField]
    float _scaleSpeed;

    public override void Initialize()
    {
        var player = FindObjectOfType<PlayerController>();
        transform.position = player.transform.position;
        MessageBroker.Default.Receive<PlayerInputEvent>()
            .Subscribe(OnPlayerInput)
            .AddTo(this);
    }

    void Update()
    {
        var s = _scaleAmount * 2 * (_scaleAmount * Mathf.Sin(_scaleSpeed * Time.time));
        transform.localScale = new Vector3(1 - s, 1 - s, 1 - s);
    }

    void OnPlayerInput(PlayerInputEvent e)
    {
        transform.position = e.NextPosition;
    }
}
