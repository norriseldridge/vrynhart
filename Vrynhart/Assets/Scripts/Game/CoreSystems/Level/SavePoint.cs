using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class SavePoint : MonoBehaviour
{
    void Start()
    {
        MessageBroker.Default.Receive<ConversationCompleteEvent>()
            .Where(e => e.Prompt.transform == transform)
            .Subscribe(_ => SaveGame())
            .AddTo(this);
    }

    void SaveGame()
    {
        var player = FindObjectOfType<PlayerController>();
        GameSaveSystem.SaveGame(player, SceneManager.GetActiveScene().name, transform.position);
    }
}
