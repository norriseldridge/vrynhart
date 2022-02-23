using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public async void OnPressContinue()
    {
        await TransitionController.TriggerTransitionAsTask();
        GameSaveSystem.LoadGame(); // reload from last save
    }

    public async void OnPressQuit()
    {
        SceneManager.LoadSceneAsync(Constants.Game.MainMenuScene);
    }
}
