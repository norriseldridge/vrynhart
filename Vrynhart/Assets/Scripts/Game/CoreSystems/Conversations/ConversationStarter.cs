using System.Collections.Generic;
using UnityEngine;
using UniRx;

// this is a generic base class, player specific one is with player logic
[RequireComponent(typeof(Collider2D))]
public class ConversationStarter : MonoBehaviour
{
    public bool IsNearPrompt => _nearPrompts.Count > 0;

    List<ConversationPrompt> _nearPrompts = new List<ConversationPrompt>();
    ConversationPrompt _currentPrompt = null;

    void OnTriggerEnter2D(Collider2D collision)
    {
        var prompt = collision.gameObject.GetComponent<ConversationPrompt>();
        if (prompt != null && prompt.enabled)
        {
            _nearPrompts.Add(prompt);
            Brokers.Default.Publish(new EnterPromptEvent());
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        var prompt = collision.gameObject.GetComponent<ConversationPrompt>();
        if (prompt != null && prompt.enabled)
        {
            _nearPrompts.Remove(prompt);
            Brokers.Default.Publish(new ExitPromptEvent());
            if (_currentPrompt != null)
            {
                Brokers.Default.Publish(new ConversationCompleteEvent(prompt));
            }
            _currentPrompt = null;
        }
    }

    protected void TriggerPrompt()
    {
        // make sure these are valid prompts still
        _nearPrompts.RemoveAll(p => p == null || !p.enabled);

        if (_nearPrompts.Count > 0)
        {
            _currentPrompt = _nearPrompts[0];
            _currentPrompt.BroadcastMessage("StartConversation");
        }
    }
}
