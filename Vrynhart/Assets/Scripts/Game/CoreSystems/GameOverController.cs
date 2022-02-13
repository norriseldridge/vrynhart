using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public void OnPressContinue()
    {
        GameSaveSystem.LoadGame(); // reload from last save
    }

    public void OnPressQuit()
    {
        SceneManager.LoadSceneAsync(Constants.Game.MainMenuScene);
    }
}
