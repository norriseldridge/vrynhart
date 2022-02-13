using System.Collections;
using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    [SerializeField]
    float _delay;

    [SerializeField]
    BloodParticle _source;

    [SerializeField]
    int _particles;

    [SerializeField]
    SpriteRenderer _splatter;
    Color _color = Color.white;

    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        _color.a = 0;
        _splatter.color = _color;
        yield return new WaitForSeconds(_delay);
        for (int i = 0; i < _particles; ++i)
            Instantiate(_source, transform.position, Quaternion.Euler(Vector3.zero));
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        _color.a = 0;
        _splatter.color = _color;

        yield return new WaitForSeconds(0.2f);

        var steps = 15.0f;
        for (int i = 0; i < steps; ++i)
        {
            _color.a = i / steps;
            _splatter.color = _color;
            yield return null;
        }
    }
}
