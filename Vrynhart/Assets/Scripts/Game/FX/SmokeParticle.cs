using UnityEngine;

public class SmokeParticle : MonoBehaviour
{
    [SerializeField]
    float _sway;

    [SerializeField]
    float _swaySpeed;

    [SerializeField]
    float _riseSpeed;

    [SerializeField]
    float _lifeTime;

    [SerializeField]
    float _scaleSpeed;

    [SerializeField]
    Vector3 _scale;

    [SerializeField]
    float _fadeSpeed;

    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    Color _color;

    float _currentSway;
    float _rand;
    float _x;
    float _life;

    void Update()
    {
        _life -= Time.deltaTime;
        if (_life > 0)
        {
            if (_currentSway < _sway)
                _currentSway += Time.deltaTime;

            transform.SetPositionAndRotation(
                new Vector3(_x + Mathf.Sin(Time.time * _swaySpeed + _rand) * _currentSway,
                    transform.position.y + _riseSpeed * Time.deltaTime),
                Quaternion.Euler(0, Time.time, 0));
            transform.localScale += _scaleSpeed * Time.deltaTime * Vector3.one;

            var color = _renderer.color;
            color.a -= _fadeSpeed * Time.deltaTime;
            _renderer.color = color;
        }
        else
            gameObject.SetActive(false);
    }

    public void Reset()
    {
        _x = transform.position.x;
        _renderer.color = _color;
        _currentSway = 0;
        transform.localScale = _scale;
        _rand = Random.value;
        _life = _lifeTime;
    }
}
