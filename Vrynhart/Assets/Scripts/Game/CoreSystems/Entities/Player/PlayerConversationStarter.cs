using UnityEngine;

public class PlayerConversationStarter : ConversationStarter
{
    void Update()
    {
        if (IsNearPrompt && Input.GetButtonDown("Submit"))
        {
            TriggerPrompt();
        }
    }
}
