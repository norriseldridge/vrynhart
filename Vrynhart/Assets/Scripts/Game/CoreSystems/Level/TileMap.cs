using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TileMap : MonoBehaviour
{
    List<Tile> _tiles;

    void Start()
    {
        _tiles = new List<Tile>(FindObjectsOfType<Tile>());

        MessageBroker.Default.Receive<TileMoveEvent>()
            .Subscribe(OnTileMove)
            .AddTo(this);

        MessageBroker.Default.Receive<TileMoveCompleteEvent>()
            .Subscribe(e => {
                if (e.Mover != null && e.Mover.ShouldPlayStepSounds)
                {
                    var tile = GetTileAt(e.Mover.transform.position);
                    tile.PlayStepSound();
                }
            })
            .AddTo(this);
    }

    public Tile GetTileAt(Vector3 position) =>
        _tiles.Find(t => Vector2.Distance(t.transform.position, position) < 1);

    void OnTileMove(TileMoveEvent e)
    {
        // is there a tile at this position?
        var targetPosition = e.Mover.transform.position + new Vector3(e.Direction.x, e.Direction.y, 0);
        var tile = GetTileAt(targetPosition);
        if (tile != null && tile.enabled && tile.IsFloor)
            e.Mover.MoveTo(tile.transform.position);
    }

    public bool IsFloorAt(Vector3 position)
    {
        var tile = GetTileAt(position);
        return tile != null && tile.enabled && tile.IsFloor;
    }
}
