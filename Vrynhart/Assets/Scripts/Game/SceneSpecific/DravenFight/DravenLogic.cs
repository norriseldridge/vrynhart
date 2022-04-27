using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DravenLogic : EnemyLogic
{
    [SerializeField]
    List<Transform> _possibleDashPoints;

    [SerializeField]
    Transform _attackIndicator;

    [SerializeField]
    AudioClip _attackSfx;

    [SerializeField]
    float _attackVolume;

    PlayerController _player;
    int _state = 0;
    Vector2 _attackPosition;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    void OnDestroy()
    {
        _attackIndicator.position = new Vector3(-30, 0);
    }

    public override void DoLogic()
    {
        if (_player == null)
            return;

        switch (_state)
        {
            case 0:
                JumpAwayFromPlayer();
                break;

            case 1: // wait
                break;

            case 2:
                TeleportToPlayer();
                break;

            case 3: // wait
                break;

            case 4:
                Attack();
                _state = -1;
                break;
        }

        ++_state;
    }

    void JumpAwayFromPlayer()
    {
        // pick the furthest
        var point = _possibleDashPoints
            .OrderByDescending(p => Vector2.Distance(p.position, _player.transform.position))
            .First();

        if (point)
            StartCoroutine(DashTo(point));
    }

    void TeleportToPlayer()
    {
        // attack sound
        Brokers.Audio.Publish(new AudioEvent(_attackSfx, _attackVolume));

        var tileMap = FindObjectOfType<TileMap>();
        _attackPosition = tileMap.GetTileAt(_player.transform.position).transform.position;
        _attackIndicator.position = _attackPosition;
        var offset = (transform.position - _player.transform.position).normalized;
        var tile = tileMap.GetTileAt(_player.transform.position + offset);
        StartCoroutine(DashTo(tile.transform));
    }

    void Attack()
    {
        // attack event
        Brokers.Default.Publish(new EnvironmentalDamageEvent(_attackPosition, 1, transform));

        // move the indicator out of the way
        _attackIndicator.position = new Vector3(-30, 0);
    }

    IEnumerator DashTo(Transform point)
    {
        EnemyController.EnemyView.SetState(EnemyVisualState.Run);
        EnemyController.EnemyView.FaceTowards(point.position);
        var steps = 30;
        var dist = Vector2.Distance(point.position, transform.position);
        for (var i = 0; i < steps; ++i)
        {
            transform.position = Vector2.MoveTowards(transform.position, point.position, dist / steps);
            yield return null;
        }
    }
}
