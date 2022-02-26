using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

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

    [Header("Options")]
    [SerializeField]
    GameObject _optionsPopup;

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


        // option sliders
        _musicVolume.onValueChanged.AddListener(OnSetMusicValue);
        _sfxVolume.onValueChanged.AddListener(OnSetSFXValue);

        // no save files exist so we can't load or continue
        if (!GameSaveSystem.DoSaveFilesExist())
        {
            _continue.gameObject.SetActive(false);
            _load.gameObject.SetActive(false);
        }

        MessageBroker.Default.Publish(new MusicEvent(_music));
        MessageBroker.Default.Publish(new AmbientAudioEvent(null));
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

    void OnDestroy()
    {
        _nameInput.onEndEdit.RemoveAllListeners();
        _musicVolume.onValueChanged.RemoveAllListeners();
        _sfxVolume.onValueChanged.RemoveAllListeners();
    }

    public async void OnClickPlay()
    {
        MessageBroker.Default.Publish(new AudioEvent(_select, _selectVolume));
        await TransitionController.TriggerTransitionAsTask();
        GameSaveSystem.LoadLastPlayedGame();
    }

    public void OnClickNewGame()
    {
        MessageBroker.Default.Publish(new AudioEvent(_select, _selectVolume));
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
                MessageBroker.Default.Publish(new TransitionEvent(TransitionType.End));
            };
    }

    public void OnCloseNewGamePopup()
    {
        _newGamePopup.SetActive(false);
    }

    public void OnClickLoadGame()
    {
        // TODO load a game???
        MessageBroker.Default.Publish(new AudioEvent(_select, _selectVolume));

        // destory children
        foreach (Transform child in _loadGamePopupContainer)
            Destroy(child.gameObject);

        _loadGamePopup.SetActive(true);

        // populate with files
        var files = GameSaveSystem.GetSaveFiles();
        foreach (var file in files)
        {
            var tile = Instantiate(_saveFileUISource, _loadGamePopupContainer);
            tile.Populate(file);
        }
    }

    public void OnCloseLoadGamePopUp()
    {
        _loadGamePopup.SetActive(false);
    }

    public void OnClickOptions()
    {
        MessageBroker.Default.Publish(new AudioEvent(_select, _selectVolume));
        _musicVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.MusicVolume, 1.0f);
        _sfxVolume.value = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f);
        _optionsPopup.SetActive(true);
    }

    public void OnCloseOptionsPopUp()
    {
        _optionsPopup.SetActive(false);
    }

    public async void OnClickQuit()
    {
        MessageBroker.Default.Publish(new AudioEvent(_select, _selectVolume));
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
