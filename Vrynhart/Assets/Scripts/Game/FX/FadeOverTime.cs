using UnityEngine;

public class FadeOverTime : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    float _speed;

    Color _color;

    void Start()
    {
        _color = _renderer.color;
    }

    void Update()
    {
        if (_color.a <= 0)
            Destroy(gameObject);

        _color.a -= _speed * Time.deltaTime;
        _renderer.color = _color;
    }
}
