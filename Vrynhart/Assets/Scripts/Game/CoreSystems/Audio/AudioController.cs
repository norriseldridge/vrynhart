using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    List<AudioSource> _pool;

    List<AudioEvent> _events = new List<AudioEvent>();
    float _fadeSpeed = 0.55f;
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

        _pool = new List<AudioSource>();
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

    void Update()
    {
        if (_events.Count == 0)
            return;

        var listener = GetListener();
        if (listener == null)
            return;

        // filter
        var filtered = new List<AudioEvent>();
        foreach (var e in _events.OrderBy(e => e.Priority))
        {
            var existing = filtered.FirstOrDefault(f => f.Clip == e.Clip);
            if (existing == null)
                filtered.Add(e);
            else
            {
                if (e.Position != null)
                {
                    if (Vector2.Distance(listener.position, e.Position.Value) < Vector2.Distance(listener.position, existing.Position.Value))
                    {
                        filtered.Remove(existing);
                        filtered.Add(e);
                    }
                }
            }
        }

        Debug.Log($"[AudioController] Events in queue: {_events.Count}  Post filter: {filtered.Count}");

        _events.Clear();

        // play the list
        while (filtered.Count > 0)
        {
            var e = filtered[0];
            filtered.RemoveAt(0);

            if (_pool.Count < _poolSize)
            {
                _pool.Add(gameObject.AddComponent<AudioSource>());
            }

            // get the first not playing source
            var next = _pool.FirstOrDefault(s => !s.isPlaying);

            // or just the next in the list
            if (next == null)
                next = _pool.FirstOrDefault();

            float distMult = 1;
            if (e.Position.HasValue)
            {
                distMult = CalculateVolume(e.Position.Value);
            }
            next.volume = _sfxVolume * e.Volume * distMult;
            next.pitch = Random.Range(e.PitchRange.x, e.PitchRange.y);
            next.PlayOneShot(e.Clip);
            _pool.Add(next);
        }
    }

    public static float CalculateVolume(Vector2 position)
    {
        var listener = GetListener();
        if (listener == null)
            return 0;

        var dist = Vector2.Distance(listener.position, position);
        var distMult = 1 - Mathf.Clamp(dist / MaxHearingDistance, 0, 1);
        return distMult;
    }

    static Transform GetListener()
    {
        Transform listener = null;
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
            listener = player.transform;
        else
        {
            var camera = FindObjectOfType<Camera>();
            if (camera != null)
                listener = camera.transform;
        }
        return listener;
    }

    void OnAudioEvent(AudioEvent e)
    {
        if (e.Clip == null)
            return;

        _events.Add(e);
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
        if (_musicSource.isPlaying && e.ShouldFade)
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

        if (clip != null)
        {
            source.clip = clip;
            source.Play();

            while (source.volume < maxVolume * currentVolume)
            {
                source.volume += _fadeSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }
}
