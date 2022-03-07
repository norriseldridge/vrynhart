using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerSliderAdjuster : MonoBehaviour
{
    [SerializeField]
    Slider _slider;

    [SerializeField]
    Image _background;

    [SerializeField]
    Color _activeColor;

    [SerializeField]
    Color _inactiveColor;

    [SerializeField]
    float _stepAmount;

    [SerializeField]
    float _selectionDelay = 0.2f;

    float _currentDelay = 0;

    public void OnSelect() => _background.color = _activeColor;

    public void OnDeselect() => _background.color = _inactiveColor;

    void Update()
    {
        if (!CustomInput.IsController())
            return;

        // allow making a change
        if (_currentDelay <= 0.0f)
        {
            if (IsDecreasingIndex())
            {
                _slider.value -= _stepAmount;
                _currentDelay = _selectionDelay;
            }

            if (IsIncreasingIndex())
            {
                _slider.value += _stepAmount;
                _currentDelay = _selectionDelay;
            }

            _slider.value = Mathf.Clamp(_slider.value, _slider.minValue, _slider.maxValue);
        }
        else
        {
            _currentDelay -= Time.deltaTime;
        }
    }

    bool IsIncreasingIndex() => CustomInput.GetAxis("Horizontal") > CustomInput.CONTROLLER_AXIS_THRESHOLD;

    bool IsDecreasingIndex() => CustomInput.GetAxis("Horizontal") < -CustomInput.CONTROLLER_AXIS_THRESHOLD;
}
