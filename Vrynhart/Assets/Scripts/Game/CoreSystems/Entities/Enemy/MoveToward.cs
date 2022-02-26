using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class MoveToward : EnemyLogic
{
    [SerializeField]
    Transform _target;

    [SerializeField]
    [Min(1)]
    int _turns = 1;

    public Transform Target
    {
        get => _target;
        set => _target = value;
    }

    Vector3 _lastTargetLocation;
    Stack<Tile> _currentPath;

    public async override void DoLogic()
    {
        UpdatePath();

        for (var i = 0; i < _turns; ++i)
        {
            if (this == null)
                return;

            if (_currentPath == null || _currentPath.Count == 0)
                UpdatePath();

            if (_currentPath != null && _currentPath.Count > 0)
            {
                if (_target != null)
                {
                    if (_target.transform.position == transform.position)
                        return;
                }

                var nextNode = _currentPath.Pop();
                if (nextNode != null)
                {
                    if (EnemyController.MoveToward(nextNode.transform.position))
                        await EnemyController.TileMover.IsMoving.Where(v => v == false).Take(1);
                }
            }
        }
    }

    void UpdatePath()
    {
        if (_target != null)
        {
            if (_lastTargetLocation != _target.position)
            {
                _lastTargetLocation = _target.position;
                _currentPath = PathFinder.Instance.BuildPath(transform.position, _target.position);
            }
        }
    }
}
