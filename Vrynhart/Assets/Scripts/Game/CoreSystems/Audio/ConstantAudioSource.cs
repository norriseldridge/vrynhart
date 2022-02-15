using UnityEngine;
using UniRx;

public class ConstantAudioSource : MonoBehaviour
{
    [SerializeField]
    AudioSource _source;

    [SerializeField]
    float _volume;

    float _currentVolume;
    float _sfxVolume;

    void Start()
    {
        _sfxVolume = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);
        _source.volume = 0;
        _source.Play();

        MessageBroker.Default.Receive<AudioSettingsChangedEvent>()
            .Subscribe(_ => {
                _sfxVolume = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);
            })
            .AddTo(this);
    }

    void Update()
    {
        _currentVolume = AudioController.CalculateVolume(transform.position) * _volume;
        _source.volume = _currentVolume * _sfxVolume;
    }
}
