using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    AudioClip _music;

    [SerializeField]
    AudioClip _select;

    [SerializeField]
    float _selectVolume;

    [SerializeField]
    Button _continue;

    [SerializeField]
    Button _load;

    [SerializeField]
    ControllerButtonListNavigation _buttonsList;

    [Header("New Game")]
    [SerializeField]
    GameObject _newGamePopup;

    [SerializeField]
    InputField _nameInput;

    [Header("Load Game")]
    [SerializeField]
    GameObject _loadGamePopup;

    [SerializeField]
    Transform _loadGamePopupContainer;

    [SerializeField]
    SaveGameTileUI _saveFileUISource;

    [SerializeField]
    ControllerButtonListNavigation _loadGameList;

    [Header("Options")]
    [SerializeField]
    GameObject _optionsPopup;

    [SerializeField]
    ControllerSliderListNavigation _optionsSlidersList;

    [SerializeField]
    Slider _musicVolume;

    [SerializeField]
    Slider _sfxVolume;

    void Start()
    {
        _newGamePopup.SetActive(false);
        _nameInput.onEndEdit.AddListener(OnEnterName);
        _loadGamePopup.SetActive(false);
        _optionsPopup.SetActive(false);

        _buttonsList.enabled = true;
        _optionsSlidersList.enabled = false;

        // option sliders
        _musicVolume.onValueChanged.AddListener(OnSetMusicValue);
        _sfxVolume.onValueChanged.AddListener(OnSetSFXValue);

        // no save files exist so we can't load or continue
        if (!GameSaveSystem.DoSaveFilesExist())
        {
            _continue.gameObject.SetActive(false);
            _load.gameObject.SetActive(false);
        }

        Brokers.Audio.Publish(new MusicEvent(_music));
        Brokers.Audio.Publish(new AmbientAudioEvent(null));
    }

    void Update()
    {
        if (CustomInput.GetKeyDown(CustomInput.Cancel))
        {
            if (_newGamePopup.activeSelf)
                OnCloseNewGamePopup();

            if (_loadGamePopup.activeSelf)
                OnCloseLoadGamePopUp();

            if (_optionsPopup.activeSelf)
                OnCloseOptionsPopUp();
        }
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

    void OnDestroy()
    {
        _nameInput.onEndEdit.RemoveAllListeners();
        _musicVolume.onValueChanged.RemoveAllListeners();
        _sfxVolume.onValueChanged.RemoveAllListeners();
    }

    void PlaySelectSound() => Brokers.Audio.Publish(new AudioEvent(_select, _selectVolume));

    public async void OnClickPlay()
    {
        PlaySelectSound();
        await TransitionController.TriggerTransitionAsTask();
        GameSaveSystem.LoadLastPlayedGame();
    }

    public void OnClickNewGame()
    {
        PlaySelectSound();
        _newGamePopup.SetActive(true);
    }

    async void OnEnterName(string name)
    {
        if (name.Length == 0)
            return;

        await TransitionController.TriggerTransitionAsTask();
        GameSaveSystem.CreateSaveFile(name);
        SceneManager.LoadSceneAsync("Intro")
            .completed += (s) => {
                Brokers.Default.Publish(new TransitionEvent(TransitionType.End));
            };
    }

    public void OnCloseNewGamePopup()
    {
        _newGamePopup.SetActive(false);
    }

    public void OnClickLoadGame()
    {
        // this button list is no longer active
        _buttonsList.enabled = false;

        PlaySelectSound();

        // destory children
        foreach (Transform child in _loadGamePopupContainer)
            Destroy(child.gameObject);

        _loadGamePopup.SetActive(true);

        // populate with files
        var files = GameSaveSystem.GetSaveFiles();
        var addedLoadTiles = new List<SaveGameTileUI>();
        foreach (var file in files)
        {
            var tile = Instantiate(_saveFileUISource, _loadGamePopupContainer);
            tile.Populate(file, OnSelectedSaveFile);
            addedLoadTiles.Add(tile);
        }
        _loadGameList.SetOverride(addedLoadTiles.Select(t => t.GetComponent<Button>()).ToList());
    }

    async void OnSelectedSaveFile(string file)
    {
        PlaySelectSound();
        await TransitionController.TriggerTransitionAsTask();
        GameSaveSystem.LoadGame(file);
    }

    public void OnCloseLoadGamePopUp()
    {
        // this button list is active again
        _buttonsList.enabled = true;
        _loadGameList.enabled = false;

        _loadGamePopup.SetActive(false);
    }

    public void OnClickOptions()
    {
        // this button list is no longer active
        _buttonsList.enabled = false;
        _optionsSlidersList.enabled = true;

        PlaySelectSound();
        _musicVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
        _sfxVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);
        _optionsPopup.SetActive(true);
    }

    public void OnCloseOptionsPopUp()
    {
        // this button list is active again
        _buttonsList.enabled = true;
        _optionsSlidersList.enabled = false;

        _optionsPopup.SetActive(false);
    }

    public async void OnClickQuit()
    {
        PlaySelectSound();
        await TransitionController.TriggerTransitionAsTask();

#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            UnityEditor.EditorApplication.ExitPlaymode();
        }
#endif
        Application.Quit();
    }
}
