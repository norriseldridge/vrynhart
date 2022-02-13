public class ConversationCompleteEvent
{
    public ConversationPrompt Prompt { get; private set; }

    public ConversationCompleteEvent(ConversationPrompt prompt)
    {
        Prompt = prompt;
    }
}
