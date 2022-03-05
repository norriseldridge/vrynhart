using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System.Collections.Generic;

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
    List<PauseScreenTabPagePair> _tabPages;

    void Start()
    {
        var player = FindObjectOfType<PlayerController>();

        foreach (var tp in _tabPages)
            tp.Page.Initialize(player);

        SelectTab("quick");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.UnloadSceneAsync(Constants.Game.PauseScene);
            _isOpen = false;
        }
    }

    public void OnClickTab(string tab) => SelectTab(tab);

    void SelectTab(string tab) =>
        _tabPages.ForEach(tp => tp.SetActive(tp.Id == tab));

    public async void OnClickQuit()
    {
        gameObject.SetActive(false);
        await TransitionController.TriggerTransitionAsTask();
        _isOpen = false;
        SceneManager.LoadSceneAsync(Constants.Game.MainMenuScene)
            .completed += _ => {
                Brokers.Default.Publish(new TransitionEvent(TransitionType.End));
            };
    }
}
