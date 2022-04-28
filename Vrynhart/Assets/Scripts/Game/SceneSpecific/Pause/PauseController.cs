using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public class PauseController : MonoBehaviour
{
    [System.Serializable]
    public class PauseScreenTabPagePair
    {
        [SerializeField]
        string _id;

        [SerializeField]
        ButtonActiveText _tabText;

        [SerializeField]
        PausePage _page;

        public string Id => _id;
        public PausePage Page => _page;

        public void SetActive(bool active)
        {
            _tabText.SetActive(active);
            _page.gameObject.SetActive(active);
            _page.enabled = active;
        }
    }

    static bool _isOpen = false;
    public static bool IsPaused => _isOpen;
    public static void Open()
    {
        if (_isOpen)
            return;

        SceneManager.LoadSceneAsync(Constants.Game.PauseScene, LoadSceneMode.Additive);
        _isOpen = true;
    }

    [SerializeField]
    ControllerButtonListNavigation _buttons;

    [SerializeField]
    List<PauseScreenTabPagePair> _tabPages;
    IReactiveProperty<int> _tabIndex = new ReactiveProperty<int>(-1);

    void Start()
    {
        _buttons.enabled = true;
        var player = FindObjectOfType<PlayerController>();

        foreach (var tp in _tabPages)
        {
            tp.Page.Initialize(player);
            tp.SetActive(false);
        }

        _tabIndex
            .Skip(1)
            .Subscribe(i =>
            {
                if (i >= 0 && i < _tabPages.Count)
                {
                    var activeId = _tabPages[i].Id;
                    _tabPages.ForEach(tp => tp.SetActive(tp.Id == activeId));
                    _buttons.enabled = false;
                }
            }).AddTo(this);
    }

    void Update()
    {
        if (CustomInput.GetKeyDown(CustomInput.Start))
            Close();

        var activePages = _tabPages.Count(p => p.Page.gameObject.activeSelf);
        _buttons.enabled = activePages == 0;
        if (CustomInput.GetKeyDown(CustomInput.Cancel) && activePages == 0)
            Close();
    }

    void Close()
    {
        SceneManager.UnloadSceneAsync(Constants.Game.PauseScene);
        _isOpen = false;
    }

    public void OnClickTab(string tab) => SelectTab(tab);

    void SelectTab(string tab)
    {
        var index = _tabPages.IndexOf(_tabPages.Where(p => p.Id == tab).FirstOrDefault());
        if (_tabIndex.Value == index)
            _tabIndex.Value = -1;

        _tabIndex.Value = index;
    }

    public async void OnClickQuit()
    {
        gameObject.SetActive(false);

        GameSaveSystem.SaveLastPlayed();

        await TransitionController.TriggerTransitionAsTask();
        _isOpen = false;
        SceneManager.LoadSceneAsync(Constants.Game.MainMenuScene)
            .completed += _ => {
                Brokers.Default.Publish(new TransitionEvent(TransitionType.End));
            };
    }
}
