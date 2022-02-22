using UnityEngine;
using UniRx;
using System.Collections;

public class DemonJump : EnemyLogic
{
    [Header("Logic")]
    [SerializeField]
    TileMap _tileMap;

    [SerializeField]
    PlayerController _player;

    [SerializeField]
    Transform _jumpIndicator;

    [SerializeField]
    float _jumpDuration;

    [SerializeField]
    float _jumpHeight;

    [Header("Audio")]
    [SerializeField]
    AudioClip _walk;

    [SerializeField]
    float _walkVolume;

    [SerializeField]
    Vector2 _walkPitch;

    Tile _currentTarget = null;
    int _turnDelay;
    int _phase = 0;

    public override void DoLogic()
    {
        ++_turnDelay;

        if (EnemyController.Health > 20)
            _phase = 0;
        else if (EnemyController.Health > 10)
            _phase = 1;
        else
            _phase = 2;

        switch (_phase)
        {
            case 0:
                Phase0();
                break;

            case 1:
                Phase1();
                break;

            case 2:
            default:
                Phase2();
                break;
        }
    }

    void Phase0()
    {
        switch (_turnDelay)
        {
            case 1: // pick a new tile (the one the player is at)
                _currentTarget = _tileMap.GetTileAt(_player.transform.position);
                _jumpIndicator.position = _currentTarget.transform.position;
                break;

            case 2:
            case 4:
            case 6:
                break;

            case 3: // wait, jump
            case 5: // wait, jump
                DoJump();
                break;

            case 7: // start the process over again
            default:
                _turnDelay = 0;
                break;
        }
    }

    void Phase1()
    {
        switch (_turnDelay)
        {
            case 1: // pick a new tile (the one the player is at)
                _currentTarget = _tileMap.GetTileAt(_player.transform.position);
                _jumpIndicator.position = _currentTarget.transform.position;
                break;

            case 2:
            case 4:
            case 6:
                break;

            case 3: // wait, jump
                DoJump();
                break;

            case 5: // start the process over again
            default:
                _turnDelay = 0;
                break;
        }
    }

    void Phase2()
    {
        switch (_turnDelay)
        {
            case 1: // pick a new tile (the one the player is at)
                _currentTarget = _tileMap.GetTileAt(_player.transform.position);
                _jumpIndicator.position = _currentTarget.transform.position;
                break;

            case 2: // jump
            case 3: // jump
            case 4: // jump
                DoJump();
                break;

            case 5: // start the process over again
            default:
                _turnDelay = 0;
                break;
        }
    }

    async void DoJump()
    {
        await StartCoroutine(JumpTo(_currentTarget.transform.position));

        MessageBroker.Default.Publish(new AudioEvent(_walk, _walkVolume * 1.2f, _walkPitch.x, _walkPitch.y, transform.position));
        MessageBroker.Default.Publish(new CameraShakeEvent(0.5f, 0.06f, 15));
    }

    IEnumerator JumpTo(Vector3 target)
    {
        EnemyController.EnemyView.FaceTowards(target);
        var initialOffset = EnemyController.EnemyView.transform.localPosition;
        var offSet = new Vector3(initialOffset.x, initialOffset.y, initialOffset.z);
        var halfWay = _jumpDuration / 2.0f;

        var step = Vector3.Distance(transform.position, target) / _jumpDuration;
        for (var t = 0.0f; t < _jumpDuration; t += Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, step * Time.deltaTime);
            offSet.y = initialOffset.y + (_jumpHeight * ((halfWay - Mathf.Abs(t - halfWay)) / halfWay));
            EnemyController.EnemyView.transform.localPosition = offSet;
            yield return null;
        }
        EnemyController.EnemyView.transform.localPosition = initialOffset;
        transform.position = target;

        // broadcast this boss has moved
        MessageBroker.Default.Publish(new TileMoveCompleteEvent(EnemyController.TileMover));
    }
}
