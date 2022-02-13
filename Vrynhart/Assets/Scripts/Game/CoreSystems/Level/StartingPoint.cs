using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StartPoint
{
    public string name;
    public Transform position;
}

public class StartingPoint : MonoBehaviour
{
    [SerializeField]
    List<StartPoint> _points;

    public List<StartPoint> Points => _points;

    public Transform GetStartLocationByName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            var point = _points.Where(p => p.name == name).First();
            if (point != null)
                return point.position;
        }

        return null;
    }
}
