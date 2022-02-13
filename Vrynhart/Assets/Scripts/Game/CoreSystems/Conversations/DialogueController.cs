using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class DialogueController : MonoBehaviour
{
    ConversationPrompt _prompt;
    string _openDialogeScene = null;

    void Awake()
    {
        DontDestroyOnLoad(this);

        MessageBroker.Default.Receive<ConversationCompleteEvent>()
            .Subscribe(OnConversationComplete)
            .AddTo(this);

        MessageBroker.Default.Receive<SimpleConversationEvent>()
            .Subscribe(OnSimpleConversation)
            .AddTo(this);
    }

    void OnConversationComplete(ConversationCompleteEvent e)
    {
        var flagId = e.Prompt.Conversation.PersistantFlagId;
        if (!string.IsNullOrEmpty(flagId))
        {
            GameSaveSystem.CacheGame(flagId);
        }

        if (_openDialogeScene != null)
        {
            SceneManager.UnloadSceneAsync(_openDialogeScene);
            _openDialogeScene = null;
        }
    }

    async void OnSimpleConversation(SimpleConversationEvent e)
    {
        if (_openDialogeScene == null)
        {
            _openDialogeScene = "SimpleDialogue";
            _prompt = e.Prompt;

            await SceneManager.LoadSceneAsync(_openDialogeScene, LoadSceneMode.Additive).AsObservable();

            var view = FindObjectOfType<SimpleDialogueView>();
            view.StartConversation(_prompt);
        }
    }
}
