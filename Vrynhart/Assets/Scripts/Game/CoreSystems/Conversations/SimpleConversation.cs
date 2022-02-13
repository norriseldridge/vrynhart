using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blood/Conversation/Simple")]
public class SimpleConversation : Conversation
{
    public string ConversationName;

    [TextArea(3, 10)]
    public List<string> Dialogue;
}