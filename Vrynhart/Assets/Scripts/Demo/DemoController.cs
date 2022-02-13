using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class DemoController : MonoBehaviour
{
    void Start()
    {
        MessageBroker.Default.Publish(new TransitionEvent(TransitionType.End));
    }

    public void OnClickQuit()
    {
        SceneManager.LoadScene(Constants.Game.MainMenuScene);
    }
}
