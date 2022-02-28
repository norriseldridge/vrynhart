using UnityEngine;

public class Swirl : MonoBehaviour
{
    [SerializeField]
    Material _material;

    [SerializeField]
    float _rotationSpeed;

    [SerializeField]
    Vector2 _swirlRange;

    [SerializeField]
    float _swirlSpeed;

    void Update()
    {
        _material.SetFloat("_Rotation", Time.time * _rotationSpeed);

        float swirlAmount = _swirlRange.x + ((_swirlRange.y - _swirlRange.x) * Mathf.Clamp(Mathf.Sin(Time.time * _swirlSpeed), 0, 1));
        _material.SetFloat("_SwirlAmount", swirlAmount);
    }
}
