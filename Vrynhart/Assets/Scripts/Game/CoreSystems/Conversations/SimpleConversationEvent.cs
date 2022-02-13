public class SimpleConversationEvent
{
    public ConversationPrompt Prompt { get; private set; }
    public SimpleConversationEvent(ConversationPrompt prompt)
    {
        Prompt = prompt;
    }
}