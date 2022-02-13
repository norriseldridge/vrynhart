using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightIntensity : MonoBehaviour
{
    [SerializeField]
    float _range;

    [SerializeField]
    float _speed;

    Light2D _light;
    float _intensity;

    void Start()
    {
        _light = GetComponent<Light2D>();
        _intensity = _light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        _light.intensity = _intensity + (_range * Mathf.Sin(Time.time * _speed));
    }
}
