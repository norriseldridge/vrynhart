using UnityEngine;

public class SnowflakeSpawn : MonoBehaviour
{
    [SerializeField]
    Transform _target;

    [SerializeField]
    Vector3 _offset;

    [SerializeField]
    Snowflake _source;

    [SerializeField]
    float _range;

    [SerializeField]
    float _delay;

    float _nextDelay = 0;
    ObjectPool<Snowflake> _pool;

    void Start()
    {
        _pool = new ObjectPool<Snowflake>(_source, 20);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _target.position + _offset;
        _nextDelay -= Time.deltaTime;
        if (_nextDelay <= 0)
        {
            _nextDelay = Random.value * _delay;
            var flake = _pool.GetNext();
            // positioning needs to happen fist
            flake.transform.position = transform.position + new Vector3(Random.Range(-_range, _range), 0);
            flake.Reset();
            flake.gameObject.SetActive(true);
        }
    }
}
