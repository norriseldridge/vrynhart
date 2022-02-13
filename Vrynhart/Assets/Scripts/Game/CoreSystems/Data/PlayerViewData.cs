using UnityEngine;
using Newtonsoft.Json;

// NOTE this is a workaround for the fact that
// Unity's Color does NOT serialize nicely with Newtonsoft Json
[System.Serializable]
public class SerializeableColor
{
    [JsonProperty]
    float[] _rgba = new float[4];

    [JsonIgnore]
    public Color Color
    {
        get => new Color(_rgba[0], _rgba[1], _rgba[2], _rgba[3]);
        set
        {
            _rgba[0] = value.r;
            _rgba[1] = value.g;
            _rgba[2] = value.b;
            _rgba[3] = value.a;
        }
    }

    public static implicit operator Color(SerializeableColor color) => color.Color;
    public static implicit operator SerializeableColor(Color color) => new SerializeableColor() { Color = color };
}

[System.Serializable]
public class PlayerViewData
{
    public SerializeableColor Head = new Color(0.849056601524353f, 0.7467150092124939f, 0.6127625703811646f);
    public SerializeableColor Hair = new Color(0.2924528121948242f, 0.16681157052516938f, 0.0179334357380867f);
    public SerializeableColor Eyes = new Color(0.05660378932952881f, 0.05660378932952881f, 0.05660378932952881f);
    public string HatId;
    public string ClothesId;
}
