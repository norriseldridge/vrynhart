using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField]
    float _duration;

    [SerializeField]
    Light2D _light;

    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        for (var i = 0.0f; i < _duration; i += Time.deltaTime)
        {
            if (_light != null)
                _light.intensity = (_duration - i) / _duration;
            yield return null;
        }
        Destroy(gameObject);
    }
}
