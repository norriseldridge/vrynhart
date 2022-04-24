using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance => FindObjectOfType<PathFinder>();

    Dictionary<Tile, TransientPathNode> _nodes;

    void Start()
    {
        _nodes = new Dictionary<Tile, TransientPathNode>();

        var tiles = FindObjectsOfType<Tile>();
        foreach (var tile in tiles)
        {
            _nodes[tile] = new TransientPathNode(tile, FindNeighbors(tile));
        }

        List<Tile> FindNeighbors(Tile tile)
        {
            List<Tile> neighbors = new List<Tile>();
            foreach (var potential in tiles)
            {
                if (tile != potential)
                {
                    var dist = Vector2.Distance(tile.transform.position, potential.transform.position);
                    if (dist <= 1)
                        neighbors.Add(potential);
                }
            }
            return neighbors;
        }
    }

    public Stack<Tile> BuildPath(Vector2 start, Vector3 end, bool mustFollowFloor = true)
    {
        var startTile = FindTileAt(start);
        var endTile = FindTileAt(end);
        return BuildPath(startTile, endTile, mustFollowFloor);
    }

    public Stack<Tile> BuildPath(Tile start, Tile end, bool mustFollowFloor)
    {
        Reset();

        Stack<Tile> result = new Stack<Tile>();

        var startNode = _nodes[start];
        startNode.LocalGoal = 0;
        startNode.GlobalGoal = Heuristic(start, end);

        if (start == end)
            return result;

        var endNode = _nodes[end];

        var toTest = new List<TransientPathNode>();
        toTest.Add(startNode);

        while (toTest.Count > 0 && endNode.Parent == null)
        {
            toTest.Sort((TransientPathNode l, TransientPathNode r) => (l.GlobalGoal < r.GlobalGoal) ? -1 : 1);

            // confirm the first is less than less
            if (toTest[0].GlobalGoal > toTest[toTest.Count - 1].GlobalGoal)
                throw new System.Exception($"Element at 0 was greater than element at {toTest.Count - 1}! Sort is wrong!");

            // don't double check nodes we've looked at before
            if (toTest[0].IsVisited)
            {
                toTest.RemoveAt(0);
                continue;
            }

            var current = toTest[0];
            current.IsVisited = true;

            foreach (var tile in current.Neighbors)
            {
                var neighbor = _nodes[tile];
                if (!neighbor.IsVisited && tile.enabled && (tile.IsFloor || !mustFollowFloor))
                    toTest.Add(neighbor);

                float possibleLowerLocalGoal = current.LocalGoal + Heuristic(current.Tile, neighbor.Tile);

                if (possibleLowerLocalGoal < neighbor.LocalGoal)
                {
                    neighbor.Parent = current;
                    neighbor.LocalGoal = possibleLowerLocalGoal;
                    neighbor.GlobalGoal = neighbor.LocalGoal + Heuristic(neighbor.Tile, end);
                }
            }
        }

        // we didn't find a path so there is no next step
        if (endNode.Parent == null)
            return result;

        var next = endNode;
        while (next.Parent != null)
        {
            result.Push(next.Tile);
            next = next.Parent;
        }

        return result;
    }

    Tile FindTileAt(Vector2 position)
    {
        foreach (var tile in _nodes.Keys)
        {
            if (Vector2.Distance(position, tile.transform.position) < 1)
                return tile;
        }

        return null;
    }

    void Reset()
    {
        foreach (var node in _nodes.Values)
            node.Reset();
    }

    float Heuristic(Tile current, Tile target)
    {
        // TODO do we account for other goals? how? dangerous tiles? etc.
        return Vector2.Distance(current.transform.position, target.transform.position);
    }
}
