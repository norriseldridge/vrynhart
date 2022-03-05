using UnityEngine;
using UniRx;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ShopPrompt : MonoBehaviour
{
    [SerializeField]
    SimpleConversation _conversation;

    [SerializeField]
    List<string> _itemListings;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Brokers.Default.Publish(new ShopEvent(true, _itemListings, _conversation));
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Brokers.Default.Publish(new ShopEvent(false));
    }
}
