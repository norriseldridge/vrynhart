using UnityEngine;

public class MusicEvent
{
    public AudioClip Clip { get; private set; }
    public float Volume { get; private set; }
    public bool ShouldFade { get; private set; }
    public MusicEvent(AudioClip clip, float volume = 1, bool shouldFade = true)
    {
        Clip = clip;
        Volume = volume;
        ShouldFade = shouldFade;
    }
}