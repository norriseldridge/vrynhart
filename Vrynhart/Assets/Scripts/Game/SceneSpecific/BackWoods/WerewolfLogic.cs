using System.Collections.Generic;
using UniRx;

public class WerewolfLogic : EnemyLogic
{
    int _state = 0;
    Stack<Tile> _currentPath;

    public override void DoLogic()
    {
        switch (_state)
        {
            case 0:
            case 2:
                BuildPath();
                MoveTowardsPlayer(3);
                break;

            // wait

            case 5:
                Brokers.Default.Publish(new WerewolfSummonEvent(transform.position));
                _state = -1;
                break;
        }

        ++_state;
    }

    void BuildPath()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player)
        {
            _currentPath = PathFinder.Instance.BuildPath(transform.position,
                player.transform.position);
        }
    }

    async void MoveTowardsPlayer(int steps)
    {
        for (var i = 0; i < steps; ++i)
        {
            if (_currentPath != null && _currentPath.Count > 0)
            {
                var nextNode = _currentPath.Pop();
                if (nextNode != null && EnemyController.MoveToward(nextNode.transform.position))
                    await EnemyController.TileMover.IsMoving.Where(v => v == false).Take(1);
            }
        }
    }
}
