using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [Header("Items")]
    [SerializeField]
    ShopListing _source;

    [SerializeField]
    Transform _container;

    [SerializeField]
    ControllerButtonListNavigation _buttons;

    [Header("Dialogue")]
    [SerializeField]
    GameObject _dialogueBox;

    [SerializeField]
    Text _npcNameText;

    [SerializeField]
    Text _dialogueText;

    [Header("Audio")]
    [SerializeField]
    AudioClip _purchaseSfx;

    [SerializeField]
    float _volume;

    PlayerController _player;

    public void OnClickClose() => Brokers.Default.Publish(new ShopEvent(false));

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    bool AttemptPurchase(ItemRecord item)
    {
        // handle one time purchases
        if (item.ItemType == ItemType.Clothes || item.ItemType == ItemType.Hat)
        {
            if (_player.Inventory.GetCount(item.Id) > 0)
                return false; // can't buy clothes if you already have them
        }

        if (_player.Inventory.GetCount("coin") >= item.Cost)
        {
            _player.Inventory.RemoveItem("coin", item.Cost);
            _player.Inventory.AddItem(item.Id, item.PurchaseQuantity);
            Brokers.Audio.Publish(new AudioEvent(_purchaseSfx, _volume));
            return true;
        }

        return false;
    }

    public void SetShopListings(List<string> itemIds)
    {
        // destory any previous listings children
        foreach (Transform child in _container)
            Destroy(child.gameObject);

        if (itemIds == null)
            return;

        if (_player == null)
            _player = FindObjectOfType<PlayerController>();

        var listings = new List<ShopListing>();
        foreach (var item in itemIds)
        {
            // handle one time purchases
            var record = ItemsLookup.GetItem(item);
            if (record.Unique)
            {
                if (_player.Inventory.GetCount(item) > 0)
                    continue;
            }

            var listing = Instantiate(_source, _container);
            listing.Initialize(_player, item);
            listing.AddOnBuyCallback(AttemptPurchase);
            listings.Add(listing);
        }
        _buttons.SetOverride(listings.Select(l => l.GetComponent<Button>()).ToList());
    }

    public void SetDialogue(SimpleConversation conversation)
    {
        if (conversation == null)
        {
            _dialogueBox.SetActive(false);
        }
        else
        {
            _npcNameText.text = conversation.ConversationName;

            // display a random line of dialogue
            var i = Random.Range(0, conversation.Dialogue.Count);
            _dialogueText.text = conversation.Dialogue[i];
            _dialogueBox.SetActive(true);
        }
    }
}
