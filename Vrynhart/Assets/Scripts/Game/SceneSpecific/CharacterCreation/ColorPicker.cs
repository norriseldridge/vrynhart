using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ColorPicker : MonoBehaviour
{
    [SerializeField]
    Slider _red;

    [SerializeField]
    Slider _green;

    [SerializeField]
    Slider _blue;

    Color _color;
    public Color Color => _color;

    void Start()
    {
        _red.onValueChanged.AddListener(OnRedChange);
        _green.onValueChanged.AddListener(OnGreenChange);
        _blue.onValueChanged.AddListener(OnBlueChange);

        _color.a = 1;
    }

    void OnRedChange(float value)
    {
        _color.r = value;
        MessageBroker.Default.Publish(new ColorChangeEvent(_color));
    }

    void OnGreenChange(float value)
    {
        _color.g = value;
        MessageBroker.Default.Publish(new ColorChangeEvent(_color));
    }

    void OnBlueChange(float value)
    {
        _color.b = value;
        MessageBroker.Default.Publish(new ColorChangeEvent(_color));
    }

    public void SetColor(Color color)
    {
        _color = color;
        _red.value = _color.r;
        _green.value = _color.g;
        _blue.value = _color.b;

        MessageBroker.Default.Publish(new ColorChangeEvent(_color));
    }
}
