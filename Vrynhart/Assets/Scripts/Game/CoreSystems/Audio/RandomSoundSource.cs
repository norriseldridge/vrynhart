using System.Collections.Generic;
using UnityEngine;

public class RandomSoundSource : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> _clips;

    [SerializeField]
    Vector2 _delay;

    [SerializeField]
    float _volume;

    float _currentDelay;

    void Start()
    {
        ResetDelay();
        PlayRandomSound();
    }

    // Update is called once per frame
    void Update()
    {
        _currentDelay -= Time.deltaTime;
        if (_currentDelay <= 0)
        {
            ResetDelay();
            PlayRandomSound();
        }
    }

    void ResetDelay() => _currentDelay = Random.Range(_delay.x, _delay.y);

    void PlayRandomSound()
    {
        var i = Random.Range(0, _clips.Count);
        Brokers.Audio.Publish(new AudioEvent(_clips[i], _volume, position: transform.position));
    }
}
