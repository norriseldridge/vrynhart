using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(EventTrigger))]
public class ButtonActiveText : MonoBehaviour
{
    [SerializeField]
    Color _activeColor;

    [SerializeField]
    Color _mouseOverColor;

    Text _text;
    Color _normalColor;

    bool _active = false;

    void Start()
    {
        _text = GetComponentInChildren<Text>();
        _normalColor = _text.color;
        UpdateText();
    }

    public void OnMouseEnter()
    {
        _text.color = _active ? _activeColor : _mouseOverColor;
    }

    public void OnMouseExit()
    {
        _text.color = _active ? _activeColor : _normalColor;
    }

    public void SetActive(bool active)
    {
        _active = active;
        UpdateText();
    }

    void UpdateText()
    {
        if (_text != null)
            _text.color = _active ? _activeColor : _normalColor;
    }
}
