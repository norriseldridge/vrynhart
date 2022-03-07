public class PlayerConversationStarter : ConversationStarter
{
    void Update()
    {
        if (IsNearPrompt && CustomInput.GetKeyDown(CustomInput.Interact))
        {
            TriggerPrompt();
        }
    }
}
