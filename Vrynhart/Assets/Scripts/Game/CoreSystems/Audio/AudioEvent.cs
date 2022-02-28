using UnityEngine;

public class AudioEvent
{
    public Vector3? Position { get; private set; }
    public AudioClip Clip { get; private set; }
    public float Volume { get; private set; }
    public Vector2 PitchRange { get; private set; }
    public int Priority { get; private set; }
    public AudioEvent(AudioClip clip, float volume = 1.0f, float lowestPitch = 1, float highestPitch = 1, Vector3? position = null, int priority = 0)
    {
        Clip = clip;
        Volume = volume;
        PitchRange = new Vector2(lowestPitch, highestPitch);
        Position = position;
        Priority = priority;
    }
}
