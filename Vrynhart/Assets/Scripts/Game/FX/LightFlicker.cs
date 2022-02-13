using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField]
    Light2D _light;

    [SerializeField]
    float _range;

    [SerializeField]
    float _speed;

    [SerializeField]
    Vector2 _flickerDelay;

    float _intensity;
    float _radius;
    float _nextFlicker;

    void Start()
    {
        _intensity = _light.intensity;
        _radius = _light.pointLightOuterRadius;
        _nextFlicker = Random.Range(_flickerDelay.x, _flickerDelay.y);
    }

    // Update is called once per frame
    void Update()
    {
        _light.intensity = _intensity * (1 + (_range * Mathf.Sin(_speed * Time.time)));
        _light.pointLightOuterRadius = _radius * (1 + (_range * Mathf.Sin(_speed * Time.time)));
        _nextFlicker -= Time.deltaTime;
        if (_nextFlicker <= 0)
        {
            _nextFlicker = Random.Range(_flickerDelay.x, _flickerDelay.y);
            _light.intensity = _intensity * (1 - _range);
        }
    }
}
