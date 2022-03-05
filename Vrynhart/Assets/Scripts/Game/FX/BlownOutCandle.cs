using UnityEngine;

public class BlownOutCandle : MonoBehaviour
{
    [SerializeField]
    AudioClip _sfx;

    [SerializeField]
    float _volume;

    void OnEnable()
    {
        Brokers.Audio.Publish(new AudioEvent(_sfx, _volume, position: transform.position));
    }
}
