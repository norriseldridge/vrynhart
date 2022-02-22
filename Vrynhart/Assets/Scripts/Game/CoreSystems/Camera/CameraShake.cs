using System.Collections;
using UnityEngine;
using UniRx;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MessageBroker.Default.Receive<CameraShakeEvent>()
            .Subscribe(OnCameraShake)
            .AddTo(this);
    }

    void OnCameraShake(CameraShakeEvent e)
    {
        StartCoroutine(Shake(e.ShakeTime, e.Intensity, e.Speed));
    }

    IEnumerator Shake(float shakeTime, float intensity, float speed)
    {
        var pos = transform.localPosition;
        for (var i = 0.0f; i < shakeTime; i += Time.deltaTime)
        {
            transform.localPosition = intensity * Mathf.Sin(Time.time * speed) * Vector3.one;
            yield return null;
        }

        transform.localPosition = pos;
    }
}
