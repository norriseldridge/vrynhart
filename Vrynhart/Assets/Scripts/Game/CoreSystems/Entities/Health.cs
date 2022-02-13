using UnityEngine;
using UniRx;

public class Health : MonoBehaviour
{
    [SerializeField]
    int _maxHealth;

    [SerializeField]
    int _health;

    public int CurrentHealth => _health;
    public float HealthPercent => _health / (float)_maxHealth;
    public bool IsAlive => _health > 0;

    void Awake()
    {
        _health = _maxHealth;
    }

    public void SetHealth(int health)
    {
        _health = health;
        MessageBroker.Default.Publish(new HealthChangeEvent(this, 0));
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health < 0)
            _health = 0;
        MessageBroker.Default.Publish(new HealthChangeEvent(this, -damage));
    }

    public void RestoreHealth(int health)
    {
        _health += health;
        if (_health > _maxHealth)
            _health = _maxHealth;
        MessageBroker.Default.Publish(new HealthChangeEvent(this, health));
    }
}
