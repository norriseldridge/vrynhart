using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BloodParticle : MonoBehaviour
{
    [SerializeField]
    float _life;
    float _maxLife;

    [SerializeField]
    SpriteRenderer _renderer;

    Rigidbody2D _rb;
    Color _color;

    void Start()
    {
        _maxLife = _life;
        _color = _renderer.color;
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(new Vector2(-1 + Random.value * 2, Random.value * 2.5f) * 50);
        _rb.AddTorque((-0.5f + Random.value) * 40);
    }

    void Update()
    {
        _rb.velocity += Vector2.down * 2 * Time.deltaTime;
        _life -= Time.deltaTime;
        _color.a = _life / _maxLife;
        _renderer.color = _color;
        if (_life < 0)
            Destroy(gameObject);
    }
}
