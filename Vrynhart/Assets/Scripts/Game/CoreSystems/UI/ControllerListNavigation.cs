using System.Collections.Generic;
using UnityEngine;

public class ControllerListNavigation<T> : MonoBehaviour
{
    [SerializeField]
    bool _isVertical = true;

    [SerializeField]
    bool _allowBumpers = false;

    [SerializeField]
    bool _onlyActive = true;

    [SerializeField]
    protected List<T> _elements;

    [SerializeField]
    float _selectionDelay = 0.2f;

    int _selectedIndex = 0;
    float _currentDelay = 0;

    List<T> _validElements;

    void OnEnable()
    {
        _validElements = new List<T>(_onlyActive ? OnlyActive() : _elements);
    }

    public void SetOverride(List<T> elements)
    {
        _elements = elements;
        _validElements = new List<T>(_onlyActive ? OnlyActive() : _elements);
        _selectedIndex = 0;
    }

    protected IEnumerable<T> GetValidElements() => _validElements;
    protected virtual IEnumerable<T> OnlyActive() => _elements;

    public T GetSelected() => _validElements[_selectedIndex];

    protected virtual void AcceptSelected() { }

    protected virtual void Update()
    {
        if (CustomInput.IsController())
        {
            if (CustomInput.GetKeyDown(CustomInput.Accept))
                AcceptSelected();

            // allow making a change
            if (_currentDelay <= 0.0f)
            {
                if (IsDecreasingIndex())
                {
                    _selectedIndex--;
                    _currentDelay = _selectionDelay;
                }

                if (IsIncreasingIndex())
                {
                    _selectedIndex++;
                    _currentDelay = _selectionDelay;
                }

                _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _validElements.Count - 1);
            }
            else
            {
                _currentDelay -= Time.deltaTime;
            }
        }
    }

    bool IsIncreasingIndex()
    {
        if (_isVertical)
            return CustomInput.GetAxis("Vertical") < -CustomInput.CONTROLLER_AXIS_THRESHOLD;

        return CustomInput.GetAxis("Horizontal") > CustomInput.CONTROLLER_AXIS_THRESHOLD || (_allowBumpers && CustomInput.GetKeyDown(CustomInput.Rb));
    }

    bool IsDecreasingIndex()
    {
        if (_isVertical)
            return CustomInput.GetAxis("Vertical") > CustomInput.CONTROLLER_AXIS_THRESHOLD;

        return CustomInput.GetAxis("Horizontal") < -CustomInput.CONTROLLER_AXIS_THRESHOLD || (_allowBumpers && CustomInput.GetKeyDown(CustomInput.Lb));
    }
}
