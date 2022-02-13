using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ShopController : MonoBehaviour
{
    [Header("Items")]
    [SerializeField]
    ShopListing _source;

    [SerializeField]
    Transform _container;

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
            _player.Inventory.AddItem(item.Id);
            MessageBroker.Default.Publish(new AudioEvent(_purchaseSfx, _volume));
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
            listing.PopulateWithItem(item);
            listing.AddOnBuyCallback(AttemptPurchase);
        }
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
