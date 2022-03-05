using UnityEngine;
using UniRx;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField]
    int _maxHealth;

    [SerializeField]
    int _health;

    [SerializeField]
    int _iFrames;

    public int CurrentHealth => _health;
    public float HealthPercent => _health / (float)_maxHealth;
    public bool IsAlive => _health > 0;
    public bool HasActiveIFrames => _currentIFrames > 0;

    int _currentIFrames;

    void Awake()
    {
        _health = _maxHealth;
    }

    public void SetHealth(int health)
    {
        _health = health;
        Brokers.Default.Publish(new HealthChangeEvent(this, 0));
    }

    public void TakeDamage(int damage)
    {
        if (HasActiveIFrames)
            return;

        StartCoroutine(RunIFrames());

        _health -= damage;
        if (_health < 0)
            _health = 0;
        Brokers.Default.Publish(new HealthChangeEvent(this, -damage));
    }

    IEnumerator RunIFrames()
    {
        for (_currentIFrames = _iFrames; _currentIFrames > 0; --_currentIFrames)
            yield return null;
        _currentIFrames = 0;
    }

    public void RestoreHealth(int health)
    {
        _health += health;
        if (_health > _maxHealth)
            _health = _maxHealth;
        Brokers.Default.Publish(new HealthChangeEvent(this, health));
    }
}
