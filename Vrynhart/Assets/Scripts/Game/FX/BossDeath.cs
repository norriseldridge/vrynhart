using System.Collections;
using UnityEngine;
using UniRx;
using UnityEngine.Experimental.Rendering.Universal;

public class BossDeath : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    AudioClip _sfx;

    [SerializeField]
    float _volume;

    [Header("Visual")]
    [SerializeField]
    Dissolver _dissolver;

    [SerializeField]
    Light2D _light;

    [SerializeField]
    float _targetIntensity;

    [SerializeField]
    float _lightSpeed;

    void Start()
    {
        MessageBroker.Default.Publish(new AudioEvent(_sfx, _volume));
        _dissolver.Dissolve();
        StartCoroutine(FadeInLight());
    }

    IEnumerator FadeInLight()
    {
        _light.intensity = 0;
        while (_light.intensity < _targetIntensity)
        {
            _light.intensity += _lightSpeed * Time.deltaTime;
            yield return null;
        }

        while (_light.intensity > 0)
        {
            _light.intensity -= _lightSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
