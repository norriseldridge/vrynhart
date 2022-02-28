using System.Linq;
using UnityEngine;

public class SlugLogic : EnemyLogic
{
    [SerializeField]
    PlayerController _player;

    public PlayerController Player { get => _player; set => _player = value; }

    int _turnDelay = 0;

    public override void DoLogic()
    {
        if (_player == null)
            return;


        var tiles = FindObjectsOfType<Tile>()
                .Where(t => t.IsFloor &&
                    Vector3.Distance(t.transform.position, transform.position) <= 1);
        Tile target = null;
        var hasLanternEquipped = _player.EquippedItem != null && _player.EquippedItem.Id == "lantern";
        if (hasLanternEquipped)
        {
            switch (_turnDelay)
            {
                case 0:
                case 2: // wait
                    break;

                case 1:
                case 3:
                case 4: // run away from the player
                    // naive solution, pick the tile furthest from player
                    target = tiles
                        .OrderByDescending(t => Vector3.Distance(t.transform.position, _player.transform.position))
                        .FirstOrDefault();
                    break;

                default:
                    _turnDelay = 0;
                    break;
            }
        }
        else
        {
            // charge the player
            target = tiles
                .OrderBy(t => Vector3.Distance(t.transform.position, _player.transform.position))
                .FirstOrDefault();
        }

        ++_turnDelay;

        if (target != null)
            EnemyController.MoveToward(target.transform.position);
    }
}
