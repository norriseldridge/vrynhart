using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    AudioSource _musicSource;

    [SerializeField]
    int _poolSize = 16;
    Queue<AudioSource> _pool;

    [SerializeField]
    [Range(1, 20)]
    float _maxHearingDistance;

    float _musicVolume;
    float _sfxVolume;
    float _currentMusicVolume = 1.0f;

    void Start()
    {
        DontDestroyOnLoad(this);

        _musicSource.loop = true;

        _musicVolume = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
        _sfxVolume = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);

        _pool = new Queue<AudioSource>();
        MessageBroker.Default.Receive<AudioEvent>()
            .Subscribe(OnAudioEvent)
            .AddTo(this);

        MessageBroker.Default.Receive<MusicEvent>()
            .Subscribe(OnMusicEvent)
            .AddTo(this);

        MessageBroker.Default.Receive<AudioSettingsChangedEvent>()
            .Subscribe(_ => {
                _musicVolume = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
                _sfxVolume = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);
                _musicSource.volume = _currentMusicVolume * _musicVolume;
            })
            .AddTo(this);
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

        var camera = FindObjectOfType<Camera>();
        float distMult = 1;
        if (e.Position.HasValue)
        {
            var dist = Vector2.Distance(camera.transform.position, e.Position.Value);
            distMult = 1 - Mathf.Clamp(dist / _maxHearingDistance, 0, 1);
        }
        next.volume = _sfxVolume * e.Volume * distMult;
        next.pitch = Random.Range(e.PitchRange.x, e.PitchRange.y);
        next.PlayOneShot(e.Clip);
        _pool.Enqueue(next);
    }

    void OnMusicEvent(MusicEvent e)
    {
        if (_musicSource.clip == e.Clip)
            return;

        _currentMusicVolume = e.Volume;
        if (_musicSource.isPlaying)
        {
            StartCoroutine(FadeOutThenPlay(e.Clip));
        }
        else
        {
            _musicSource.volume = _musicVolume * _currentMusicVolume;
            _musicSource.clip = e.Clip;
            _musicSource.Play();
        }
    }

    IEnumerator FadeOutThenPlay(AudioClip clip)
    {
        while (_musicSource.volume > 0)
        {
            _musicSource.volume -= Time.deltaTime;
            yield return null;
        }

        _musicSource.clip = clip;
        _musicSource.Play();

        while (_musicSource.volume < _musicVolume * _currentMusicVolume)
        {
            _musicSource.volume += Time.deltaTime;
            yield return null;
        }
    }
}
