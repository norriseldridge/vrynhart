using UnityEngine;

public class LevelAmbientSoundController : MonoBehaviour
{
    [SerializeField]
    AudioClip _sound;

    [SerializeField]
    float _volume;

    void Start()
    {
        Brokers.Audio.Publish(new AmbientAudioEvent(_sound, _volume));
    }
}
