using UnityEngine;
using UniRx;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ShopPrompt : RequiresPrompt
{
    [SerializeField]
    SimpleConversation _conversation;

    [SerializeField]
    List<string> _itemListings;

    bool _opened = false;

    void Start()
    {
        Brokers.Default.Receive<ShopEvent>()
            .Subscribe(e => _opened = e.Open)
            .AddTo(this);
    }

    void Update()
    {
        if (PromptUser == null)
            return;

        if (CustomInput.GetKeyDown(CustomInput.Interact) && !_opened)
            Brokers.Default.Publish(new ShopEvent(true, _itemListings, _conversation));

        if (CustomInput.GetKeyDown(CustomInput.Cancel) && _opened)
            Brokers.Default.Publish(new ShopEvent(false));

        var player = PromptUser.gameObject.GetComponent<PlayerController>();
        if (player)
            player.enabled = !_opened;
    }
}
