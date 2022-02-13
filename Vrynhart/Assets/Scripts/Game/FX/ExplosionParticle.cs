using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ExplosionParticle : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D _rb;

    [SerializeField]
    float _force;

    [SerializeField]
    GameObject _trailSource;

    [SerializeField]
    float _time;

    float _nextTime;

    void Start()
    {
        _nextTime = _time;
    }

    public void Shoot(float angle)
    {
        var f = _force * 0.5f + (Random.value * _force * 0.5f);
        _rb.AddForce(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * f, ForceMode2D.Impulse);
        _rb.AddTorque(Random.value * _force);
    }

    void Update()
    {
        _nextTime -= Time.deltaTime;
        if (_nextTime <= 0)
        {
            _nextTime = _time;
            var trail = Instantiate(_trailSource);
            trail.transform.position = transform.position;

            var r = trail.GetComponent<SpriteRenderer>();
            if (r)
            {
                r.color = GetComponent<SpriteRenderer>().color;
            }
        }

        _rb.velocity += Vector2.down * Time.deltaTime;
    }
}
