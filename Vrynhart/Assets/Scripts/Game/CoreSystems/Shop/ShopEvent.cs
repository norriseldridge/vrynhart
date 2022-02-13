using System.Collections.Generic;

public class ShopEvent
{
    public bool Open { get; private set; }
    public SimpleConversation Conversation { get; set; }
    public List<string> ItemIds { get; set; }
    public ShopEvent(bool open, List<string> itemIds = null, SimpleConversation conversation = null)
    {
        Open = open;
        ItemIds = itemIds;
        Conversation = conversation;
    }
}
