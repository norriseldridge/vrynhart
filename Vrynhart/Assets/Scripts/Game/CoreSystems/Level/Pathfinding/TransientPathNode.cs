using System.Collections.Generic;

public class TransientPathNode
{
    public bool IsVisited { get; set; }
    public float LocalGoal { get; set; }
    public float GlobalGoal { get; set; }
    public TransientPathNode Parent { get; set; }
    public Tile Tile { get; private set; }
    public List<Tile> Neighbors { get; private set; }

    public TransientPathNode(Tile tile, List<Tile> neighbors)
    {
        Tile = tile;
        Neighbors = neighbors;
        Reset();
    }

    public void Reset()
    {
        LocalGoal = float.PositiveInfinity;
        GlobalGoal = 0;
        IsVisited = false;
        Parent = null;
    }
}
