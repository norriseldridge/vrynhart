using UnityEngine;
using UnityEngine.UI;

public class OptionsPage : PausePage
{
    [SerializeField]
    ControllerSliderListNavigation _sliders;

    [SerializeField]
    Slider _musicVolume;

    [SerializeField]
    Slider _sfxVolume;

    public override void Initialize(PlayerController player)
    {
        _sliders.enabled = false;

        // set the options values
        _musicVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
        _sfxVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);

        // option sliders listeners
        _musicVolume.onValueChanged.AddListener(OnSetMusicValue);
        _sfxVolume.onValueChanged.AddListener(OnSetSFXValue);
    }

    void OnEnable()
    {
        _sliders.enabled = true;
        var children = _sliders.transform.GetComponentsInChildren<ControllerSliderAdjuster>();
        foreach (var child in children)
            child.enabled = true;
    }

    void OnDisable()
    {
        _sliders.enabled = false;
        var children = _sliders.transform.GetComponentsInChildren<ControllerSliderAdjuster>();
        foreach (var child in children)
            child.enabled = false;
    }

    void OnDestroy()
    {
        _musicVolume.onValueChanged.RemoveAllListeners();
        _sfxVolume.onValueChanged.RemoveAllListeners();
    }

    void OnSetMusicValue(float value)
    {
        PlayerPrefs.SetFloat(Constants.Prefs.MusicVolume, value);
        Brokers.Audio.Publish(new AudioSettingsChangedEvent());
    }

    void OnSetSFXValue(float value)
    {
        PlayerPrefs.SetFloat(Constants.Prefs.SFXVolume, value);
        Brokers.Audio.Publish(new AudioSettingsChangedEvent());
    }
}
