using UnityEngine;
using UniRx;

[RequireComponent(typeof(PlayerController))]
public class PlayerItemConsumer : MonoBehaviour
{
    PlayerController _player;
    float _oilTimer = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerController>();

        MessageBroker.Default.Receive<UseItemEvent>()
            .Where(IsItemUseable)
            .Subscribe(e => UseItem(e.Item))
            .AddTo(this);
    }

    void Update()
    {
        if (_player.EquippedItem != null && _player.EquippedItem.Id == "lantern")
        {
            // consume oil
            if (_player.Inventory.GetCount("oil") > 0)
            {
                _oilTimer -= Time.deltaTime;
                if (_oilTimer <= 0)
                {
                    _player.Inventory.RemoveItem("oil");
                    _oilTimer = 1.0f;
                }
            }
        }
    }

    bool IsItemUseable(UseItemEvent e)
    {
        // we don't own this item
        if (_player.Inventory.GetCount(e.Item.Id) <= 0)
            return false;

        // item isn't useable if the range is wrong
        if (Vector2.Distance(transform.position, e.TargetPosition) > e.Item.UseRange)
            return false;

        return true;
    }

    void UseItem(ItemRecord item)
    {
        // try to use the item
        switch (item.Id)
        {
            case "apple":
                _player.Inventory.RemoveItem(item.Id);
                _player.Health.RestoreHealth(1);
                break;

            case "whiskey":
                _player.Inventory.RemoveItem(item.Id);
                _player.Health.RestoreHealth(2);
                break;
        }
    }
}
