using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ConversationPrompt : MonoBehaviour
{
    [SerializeField]
    Conversation _conversation;

    [Header("Audio")]
    [SerializeField]
    AudioClip _sfx;

    [SerializeField]
    float _volume;

    public Conversation Conversation => _conversation;

    public AudioClip SFX => _sfx;
    public float Volume => _volume;

    void StartConversation()
    {
        if (_conversation is SimpleConversation)
            Brokers.Default.Publish(new SimpleConversationEvent(this));
        else
            throw new System.Exception($"Don't know how to handle converstaion of type `{_conversation.GetType()}`.");
    }
}
