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
        var offset = new Vector3(1, 1, transform.localPosition.z);
        var pos = transform.localPosition;
        for (var i = 0.0f; i < shakeTime; i += Time.deltaTime)
        {
            var o = intensity * Mathf.Sin(Time.time * speed);
            offset.x = offset.y = o;
            transform.localPosition = offset;
            yield return null;
        }

        transform.localPosition = pos;
    }
}
