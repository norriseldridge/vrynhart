using UnityEngine;
using UniRx;

public class CombatController : MonoBehaviour
{
    bool _shouldResolve = false;
    void Start()
    {
        MessageBroker.Default.Receive<TileMoveCompleteEvent>()
            .Subscribe(_ => _shouldResolve = true)
            .AddTo(this);

        MessageBroker.Default.Receive<UseItemEvent>()
            .Subscribe(OnUseItem)
            .AddTo(this);

        MessageBroker.Default.Receive<EnvironmentalDamageEvent>()
            .Subscribe(OnEnvironmentalDamageEvent)
            .AddTo(this);
    }

    void OnUseItem(UseItemEvent e)
    {
        // are we in the use range?
        if (Vector2.Distance(e.CastPosition, e.TargetPosition) > e.Item.UseRange)
            return;

        var player = FindObjectOfType<PlayerController>();
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            if (Vector2.Distance(e.TargetPosition, enemy.transform.position) < 1.0f)
            {
                if (TryUseItemOnEnemy(player, e.Item, enemy))
                {
                    player.Inventory.RemoveItem(e.Item.Id);
                    enemy.Kill();
                }
            }
        }
    }

    void OnEnvironmentalDamageEvent(EnvironmentalDamageEvent e)
    {
        var healths = FindObjectsOfType<Health>();
        foreach (var health in healths)
        {
            if (Vector2.Distance(e.Position, health.transform.position) < 1.0f)
            {
                health.TakeDamage(e.Damage);
            }
        }
    }

    void Update()
    {
        if (!_shouldResolve)
            return;
        _shouldResolve = false;

        var enemies = FindObjectsOfType<EnemyController>();
        var player = FindObjectOfType<PlayerController>();

        if (player != null && player.Health.IsAlive)
        {
            foreach (var enemy in enemies)
            {
                if (InCombatRange(player.transform, enemy.transform))
                    player.Health.TakeDamage(1);
            }
        }
    }

    bool InCombatRange(Transform t1, Transform t2) =>
        Vector2.Distance(t1.position, t2.position) < 0.5f;

    bool TryUseItemOnEnemy(PlayerController player, ItemRecord item, EnemyController enemy)
    {
        // if we even have the item
        if (item != null && player.Inventory.GetCount(item.Id) > 0)
        {
            // and this item is the one used to kill this enemy
            if (enemy.KillItemId == item.Id)
            {
                return true;
            }
        }

        return false;
    }
}
