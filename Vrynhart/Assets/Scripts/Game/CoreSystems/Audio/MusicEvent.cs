using UnityEngine;

public class MusicEvent
{
    public AudioClip Clip { get; private set; }
    public float Volume { get; private set; }
    public MusicEvent(AudioClip clip, float volume = 1)
    {
        Clip = clip;
        Volume = volume;
    }
}