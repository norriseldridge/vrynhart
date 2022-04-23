using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MainHall3 : MonoBehaviour
{
    [SerializeField]
    string _persistentUID;

    [SerializeField]
    EnemyController _barrierEnemy;

    [SerializeField]
    List<Tile> _toEnable;

    void Start()
    {
        // if we've done this before, destory the enemy and enable the tiles
        var data = GameSaveSystem.GetCachedSaveData();
        if (data.CompletedFlags.Contains(_persistentUID))
        {
            Destroy(_barrierEnemy.gameObject);
            _toEnable.ForEach(e => e.enabled = true);
        }
        else
        {
            // else listen for killing the enemy and disable the tiles
            _toEnable.ForEach(e => e.enabled = false);
            Brokers.Default.Receive<EnemyDiedEvent>()
                .Where(e => e.EnemyController == _barrierEnemy)
                .Subscribe(OnEnemyDiedEvent)
                .AddTo(this);
        }
    }

    void OnEnemyDiedEvent(EnemyDiedEvent e)
    {
        _toEnable.ForEach(e => e.enabled = true);
        GameSaveSystem.CacheGame(_persistentUID);
    }
}
