using UnityEngine;

public class AmbientAudioEvent
{
    public AudioClip Clip { get; private set; }
    public float Volume { get; private set; }
    public AmbientAudioEvent(AudioClip clip, float volume = 1)
    {
        Clip = clip;
        Volume = volume;
    }
}
