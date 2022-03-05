using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoController : MonoBehaviour
{
    void Start()
    {
        Brokers.Default.Publish(new TransitionEvent(TransitionType.End));
    }

    public void OnClickQuit()
    {
        SceneManager.LoadScene(Constants.Game.MainMenuScene);
    }
}
