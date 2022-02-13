using UnityEngine;

public class ColorChangeEvent
{
    Color _color;
    public Color Color => _color;

    public ColorChangeEvent(Color color) => _color = color;
}
