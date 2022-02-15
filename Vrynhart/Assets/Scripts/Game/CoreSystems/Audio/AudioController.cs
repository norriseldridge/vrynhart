using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AudioController : MonoBehaviour
{
    const float MaxHearingDistance = 8.0f;

    [SerializeField]
    AudioSource _musicSource;

    [SerializeField]
    AudioSource _ambientSource;

    [SerializeField]
    int _poolSize = 16;
    Queue<AudioSource> _pool;

    float _fadeSpeed = 0.5f;
    float _musicVolume;
    float _sfxVolume;
    float _currentMusicVolume = 1.0f;
    float _currentAmbientVolume = 1.0f;

    void Start()
    {
        DontDestroyOnLoad(this);

        _musicSource.loop = true;
        _ambientSource.loop = true;

        _musicVolume = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
        _sfxVolume = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);

        _pool = new Queue<AudioSource>();
        MessageBroker.Default.Receive<AudioEvent>()
            .Subscribe(OnAudioEvent)
            .AddTo(this);

        MessageBroker.Default.Receive<AmbientAudioEvent>()
            .Subscribe(OnAmbientAudioEvent)
            .AddTo(this);

        MessageBroker.Default.Receive<MusicEvent>()
            .Subscribe(OnMusicEvent)
            .AddTo(this);

        MessageBroker.Default.Receive<AudioSettingsChangedEvent>()
            .Subscribe(_ => {
                _musicVolume = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
                _sfxVolume = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);
                _musicSource.volume = _currentMusicVolume * _musicVolume;
                _ambientSource.volume = _currentAmbientVolume * _sfxVolume;
            })
            .AddTo(this);
    }

    public static float CalculateVolume(Vector2 position)
    {
        var camera = FindObjectOfType<Camera>();
        var dist = Vector2.Distance(camera.transform.position, position);
        var distMult = 1 - Mathf.Clamp(dist / MaxHearingDistance, 0, 1);
        return distMult;
    }

    void OnAudioEvent(AudioEvent e)
    {
        if (e.Clip == null)
            return;

        if (_pool.Count < _poolSize)
        {
            _pool.Enqueue(gameObject.AddComponent<AudioSource>());
        }

        var next = _pool.Dequeue();

        float distMult = 1;
        if (e.Position.HasValue)
        {
            distMult = CalculateVolume(e.Position.Value);
        }
        next.volume = _sfxVolume * e.Volume * distMult;
        next.pitch = Random.Range(e.PitchRange.x, e.PitchRange.y);
        next.PlayOneShot(e.Clip);
        _pool.Enqueue(next);
    }

    void OnAmbientAudioEvent(AmbientAudioEvent e)
    {
        if (_ambientSource.clip == e.Clip)
            return;

        _currentAmbientVolume = e.Volume;
        if (_ambientSource.isPlaying)
        {
            StartCoroutine(FadeOutThenPlay(_ambientSource, e.Clip, _sfxVolume, _currentAmbientVolume));
        }
        else
        {
            _ambientSource.volume = _sfxVolume * _currentAmbientVolume;
            _ambientSource.clip = e.Clip;
            _ambientSource.Play();
        }
    }

    void OnMusicEvent(MusicEvent e)
    {
        if (_musicSource.clip == e.Clip)
            return;

        _currentMusicVolume = e.Volume;
        if (_musicSource.isPlaying)
        {
            StartCoroutine(FadeOutThenPlay(_musicSource, e.Clip, _musicVolume, _currentMusicVolume));
        }
        else
        {
            _musicSource.volume = _musicVolume * _currentMusicVolume;
            _musicSource.clip = e.Clip;
            _musicSource.Play();
        }
    }

    IEnumerator FadeOutThenPlay(AudioSource source, AudioClip clip, float maxVolume, float currentVolume)
    {
        while (source.volume > 0)
        {
            source.volume -= _fadeSpeed * Time.deltaTime;
            yield return null;
        }

        source.clip = clip;
        source.Play();

        while (source.volume < maxVolume * currentVolume)
        {
            source.volume += _fadeSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
