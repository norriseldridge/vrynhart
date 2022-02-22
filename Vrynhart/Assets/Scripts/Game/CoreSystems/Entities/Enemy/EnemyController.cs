using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

[RequireComponent(typeof(TileMover))]
public class EnemyController : MonoBehaviour
{
    [System.Serializable]
    public class KillItem
    {
        public string ItemId;
        public int Damage;
    }

    [SerializeField]
    EnemyView _view;

    [SerializeField]
    List<EnemyLogic> _logic;

    [Header("Kill")]
    [SerializeField]
    List<KillItem> _killItems;

    [SerializeField]
    int _health;

    [SerializeField]
    AudioClip[] _hit;

    [SerializeField]
    float _hitVolume;

    [SerializeField]
    Vector2 _hitPitchRange;

    [SerializeField]
    GameObject _deathSource;

    [SerializeField]
    AudioClip _die;

    [SerializeField]
    float _dieVolume;

    public EnemyView EnemyView => _view;
    public TileMover TileMover => _mover;
    public bool IsDead => _health <= 0;
    public int Health => _health;
    public List<KillItem> KillItems => _killItems;

    TileMover _mover;
    List<Vector3> _takenPositions = new List<Vector3>();

    int _logicIndex = 0;

    void Start()
    {
        _mover = GetComponent<TileMover>();
        MessageBroker.Default.Receive<TurnProgressionEvent>()
            .Subscribe(OnTurnProgression)
            .AddTo(this);

        MessageBroker.Default.Receive<EnemyDestinationPickedEvent>()
            .Subscribe(OnEnemyDestinationPickedEvent)
            .AddTo(this);
    }

    void OnTurnProgression(TurnProgressionEvent e)
    {
        if (!enabled)
            return;

        if (!gameObject.activeSelf)
            return;

        if (_logic.Count == 0)
            return;

        if (_logicIndex >= _logic.Count)
            _logicIndex = 0;

        _logic[_logicIndex].DoLogic();
        ++_logicIndex;

        _takenPositions.Clear();
    }

    void OnEnemyDestinationPickedEvent(EnemyDestinationPickedEvent e)
    {
        if (e.EnemyController == this)
            return;

        _takenPositions.Add(e.Target);
    }

    public bool MoveToward(Vector3 target)
    {
        if (_takenPositions.Any(p => Vector2.Distance(p, target) < 1))
            return false; // another enemy is at this position, don't try

        MessageBroker.Default.Publish(new EnemyDestinationPickedEvent(this, target));

        _view.FaceTowards(target);
        var direction = target - transform.position;
        _mover.TryMove(direction);
        return true;
    }

    void Update()
    {
        _view.SetState(_mover.IsMoving ? EnemyVisualState.Run : EnemyVisualState.Idle);
    }

    public void DealDamage(string itemId)
    {
        var item = KillItems.Where(i => i.ItemId == itemId).FirstOrDefault();
        if (item != null)
            DealDamage(item.Damage);
    }

    public void DealDamage(int damage)
    {
        _health -= damage;
        MessageBroker.Default.Publish(new EnemyTakeDamageEvent(this, damage));
        if (_health <= 0)
        {
            MessageBroker.Default.Publish(new EnemyDiedEvent(this));

            // play visuals
            MessageBroker.Default.Publish(new AudioEvent(_die, _dieVolume));
            if (_deathSource != null)
                Instantiate(_deathSource, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(gameObject);
        }
        else
        {
            if (_hit.Length > 0)
                MessageBroker.Default.Publish(new AudioEvent(_hit[Random.Range(0, _hit.Length)], _hitVolume, _hitPitchRange.x, _hitPitchRange.y));
        }
    }
}
