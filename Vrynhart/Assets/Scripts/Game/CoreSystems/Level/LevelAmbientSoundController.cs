using UnityEngine;
using UniRx;

public class LevelAmbientSoundController : MonoBehaviour
{
    [SerializeField]
    AudioClip _sound;

    [SerializeField]
    float _volume;

    void Start()
    {
        MessageBroker.Default.Publish(new AmbientAudioEvent(_sound, _volume));
    }
}
