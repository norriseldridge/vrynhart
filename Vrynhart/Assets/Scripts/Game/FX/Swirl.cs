using System.Collections;
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

    public IEnumerator FadeAlpha(float start, float end, float speed)
    {
        _material.SetFloat("_Alpha", start);
        float t = start;
        while (Mathf.Abs(end - t) > 0.1f)
        {
            t = Mathf.LerpUnclamped(t, end, speed * Time.deltaTime);
            _material.SetFloat("_Alpha", t);
            yield return null;
        }
    }

    void Update()
    {
        _material.SetFloat("_Rotation", Time.time * _rotationSpeed);

        float swirlAmount = _swirlRange.x + ((_swirlRange.y - _swirlRange.x) * Mathf.Clamp(Mathf.Sin(Time.time * _swirlSpeed), 0, 1));
        _material.SetFloat("_SwirlAmount", swirlAmount);
    }
}
