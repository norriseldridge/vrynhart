using UnityEngine;

public class AmbientAudioEvent
{
    public AudioClip Clip { get; private set; }
    public float Volume { get; private set; }
    public bool ShouldFade { get; private set; }
    public AmbientAudioEvent(AudioClip clip, float volume = 1, bool shouldFade = true)
    {
        Clip = clip;
        Volume = volume;
        ShouldFade = shouldFade;
    }
}
