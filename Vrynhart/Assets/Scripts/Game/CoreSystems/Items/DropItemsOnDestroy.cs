using System.Collections.Generic;
using UnityEngine;

public class DropItemsOnDestroy : MonoBehaviour
{
    [SerializeField]
    List<LevelItem> _potentialItemDrops;

    [SerializeField]
    [Range(0, 1)]
    float _dropPercentage;

    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        if (_potentialItemDrops.Count > 0)
        {
            var r = Random.value;
            if (_dropPercentage >= r)
            {
                var i = Random.Range(0, _potentialItemDrops.Count);
                Instantiate(_potentialItemDrops[i], transform.position, Quaternion.Euler(0, 0, 0));
            }
        }
    }
}
