using UnityEngine;

public class SmokeSpawner : MonoBehaviour
{
    [SerializeField]
    SmokeParticle _source;

    [SerializeField]
    float _range;

    [SerializeField]
    float _delay;

    float _nextDelay = 0;

    ObjectPool<SmokeParticle> _pool;

    // Start is called before the first frame update
    void Start()
    {
        _pool = new ObjectPool<SmokeParticle>(_source, 20);
    }

    // Update is called once per frame
    void Update()
    {
        _nextDelay -= Time.deltaTime;
        if (_nextDelay <= 0)
        {
            _nextDelay = Random.value * _delay;

            var smoke = _pool.GetNext();
            // positioning needs to happen fist
            smoke.transform.position = transform.position + new Vector3(Random.Range(-_range, _range), 0);
            smoke.Reset();
            smoke.gameObject.SetActive(true);
        }
    }
}
