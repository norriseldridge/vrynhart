using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ItemRequired : PostLevelInitialize
{
    [SerializeField]
    string _itemId;

    [SerializeField]
    List<MonoBehaviour> _haveItemActive;

    [SerializeField]
    List<GameObject> _haveItemActiveGameObjects;

    [SerializeField]
    List<MonoBehaviour> _noItemActive;

    [SerializeField]
    List<GameObject> _noItemActiveGameObjects;

    public override void Initialize()
    {
        var player = FindObjectOfType<PlayerController>();
        ProcessInventory(player.Inventory);

        MessageBroker.Default.Receive<InventoryChangeEvent>()
            .Subscribe(OnInventoryChange)
            .AddTo(this);
    }

    void OnInventoryChange(InventoryChangeEvent e) => ProcessInventory(e.Inventory);

    void ProcessInventory(Inventory inventory)
    {
        var hasItem = inventory.GetCount(_itemId) > 0;

        // NOTE if this doesn't seem to work, know that
        // OnTrigger methods are called on MonoBehaviours even when "disabled"
        // you'll have to add an explicit "enable == true" type check in that script
        foreach (var m in _haveItemActive)
            m.enabled = hasItem;

        foreach (var go in _haveItemActiveGameObjects)
            go.SetActive(hasItem);

        foreach (var m in _noItemActive)
            m.enabled = !hasItem;

        foreach (var go in _noItemActiveGameObjects)
            go.SetActive(!hasItem);
    }
}
