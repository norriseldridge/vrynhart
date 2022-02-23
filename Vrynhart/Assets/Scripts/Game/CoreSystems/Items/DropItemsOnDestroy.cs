using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropItemsOnDestroy : MonoBehaviour
{
    [SerializeField]
    List<LevelItem> _potentialItemDrops;

    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        if (_potentialItemDrops.Count > 0)
        {
            var player = FindObjectOfType<PlayerController>();
            var inventory = player.Inventory;
            CalculateDropItem(inventory);
        }
    }

    void CalculateDropItem(Inventory inventory)
    {
        // let's start with determining what the player needs most
        var itemScores = new List<(int index, float score)>();
        for (var i = 0; i < _potentialItemDrops.Count; ++i)
        {
            var item = _potentialItemDrops[i];
            if (Constants.Game.TargetItemQuantities.TryGetValue(item.ItemId, out var target))
            {
                var count = inventory.GetCount(item.ItemId);
                var score = (count - target) / (float)target;
                itemScores.Add((i, score));
            }
        }

        if (itemScores.Count > 0)
        {
            var (index, score) = itemScores.OrderBy(p => p.score).FirstOrDefault();
            var r = Random.value;
            if (score < r) // we are below the target amount
                Instantiate(_potentialItemDrops[index], transform.position, Quaternion.Euler(0, 0, 0));
        }
    }
}
