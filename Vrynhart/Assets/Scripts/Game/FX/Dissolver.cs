using System.Collections;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    [SerializeField]
    Material _material;

    [SerializeField]
    Vector2 _fadePoints;

    [SerializeField]
    float _fadeSpeed;

    float _current;

    public void Dissolve() => StartCoroutine(DoDissolve());

    IEnumerator DoDissolve()
    {
        _current = _fadePoints.x;
        while (_current < _fadePoints.y)
        {
            _current = Mathf.Lerp(_current, _fadePoints.y, _fadeSpeed * Time.deltaTime);
            _material.SetFloat("_DissolveAmount", _current);
            yield return null;
        }
    }
}
