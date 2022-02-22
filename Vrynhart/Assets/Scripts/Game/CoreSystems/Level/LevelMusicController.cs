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
        MessageBroker.Default.Publish(new MusicEvent(_music, _volume));

        MessageBroker.Default.Receive<RestartLevelMusicEvent>()
            .Subscribe(_ => MessageBroker.Default.Publish(new MusicEvent(_music, _volume)))
            .AddTo(this);
    }
}
