using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class OptionsPage : PausePage
{
    [SerializeField]
    Slider _musicVolume;

    [SerializeField]
    Slider _sfxVolume;

    public override void Initialize(PlayerController player)
    {
        // set the options values
        _musicVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
        _sfxVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);

        // option sliders listeners
        _musicVolume.onValueChanged.AddListener(OnSetMusicValue);
        _sfxVolume.onValueChanged.AddListener(OnSetSFXValue);
    }

    void OnDestroy()
    {
        _musicVolume.onValueChanged.RemoveAllListeners();
        _sfxVolume.onValueChanged.RemoveAllListeners();
    }

    void OnSetMusicValue(float value)
    {
        PlayerPrefs.SetFloat(Constants.Prefs.MusicVolume, value);
        MessageBroker.Default.Publish(new AudioSettingsChangedEvent());
    }

    void OnSetSFXValue(float value)
    {
        PlayerPrefs.SetFloat(Constants.Prefs.SFXVolume, value);
        MessageBroker.Default.Publish(new AudioSettingsChangedEvent());
    }
}
