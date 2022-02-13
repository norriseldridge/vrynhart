using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    ExplosionParticle _source;

    [SerializeField]
    int _count;

    [SerializeField]
    Light2D _light;

    void Start()
    {
        StartCoroutine(TweenLight());

        for (int i = 0; i < _count; ++i)
        {
            var p = Instantiate(_source);
            p.transform.position = transform.position;

            p.Shoot(Random.value * Mathf.PI * 2);
        }
    }

    IEnumerator TweenLight()
    {
        if (_light == null)
            yield break;

        for (int i = 0; i < 30; ++i)
        {
            _light.intensity = i / 30.0f;
            _light.pointLightOuterRadius = (i / 30.0f) * 3;
            yield return null;
        }

        for (int i = 0; i < 60; ++i)
        {
            _light.intensity = 1 - (i / 60.0f);
            _light.pointLightOuterRadius += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
