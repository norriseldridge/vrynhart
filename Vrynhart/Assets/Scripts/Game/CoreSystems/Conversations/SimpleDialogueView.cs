using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SimpleDialogueView : BaseDialogeView
{
    [SerializeField]
    Text _npcNameText;

    [SerializeField]
    Text _text;

    ConversationPrompt _prompt;

    public override void StartConversation(ConversationPrompt prompt)
    {
        _prompt = prompt;

        if (_prompt.SFX != null)
        {
            Brokers.Audio.Publish(new AudioEvent(_prompt.SFX, _prompt.Volume));
        }

        StartCoroutine(DisplayDialogue(_prompt.Conversation as SimpleConversation));
    }

    IEnumerator DisplayDialogue(SimpleConversation conversation)
    {
        _npcNameText.text = conversation.ConversationName;

        foreach (string line in conversation.Dialogue)
        {
            _text.text = line;

            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return new WaitForEndOfFrame();
        }

        Brokers.Default.Publish(new ConversationCompleteEvent(_prompt));
    }
}
