using UnityEngine;
using UniRx;

public class BlownOutCandle : MonoBehaviour
{
    [SerializeField]
    AudioClip _sfx;

    [SerializeField]
    float _volume;

    void OnEnable()
    {
        MessageBroker.Default.Publish(new AudioEvent(_sfx, _volume, position: transform.position));
    }
}
