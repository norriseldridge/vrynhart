using UnityEngine;
using UniRx;

public class LevelMusicController : PostLevelInitialize
{
    [SerializeField]
    AudioClip _music;

    [SerializeField]
    float _volume = 1;

    public override void Initialize()
    {
        Brokers.Audio.Publish(new MusicEvent(_music, _volume));

        Brokers.Audio.Receive<RestartLevelMusicEvent>()
            .Subscribe(_ => Brokers.Audio.Publish(new MusicEvent(_music, _volume)))
            .AddTo(this);
    }
}
