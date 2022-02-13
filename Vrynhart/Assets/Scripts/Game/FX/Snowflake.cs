using UnityEngine;

public class Snowflake : MonoBehaviour
{
    [SerializeField]
    float _sway;

    [SerializeField]
    float _gravity;

    [SerializeField]
    float _lifeTime;

    float _rand;
    float _x;
    float _life;

    void Update()
    {
        _life -= Time.deltaTime;
        if (_life > 0)
            transform.position = new Vector3(_x + Mathf.Sin(Time.time + _rand) * _sway, transform.position.y - _gravity * Time.deltaTime);
        else
            gameObject.SetActive(false);
    }

    public void Reset()
    {
        _x = transform.position.x;
        _rand = Random.value;
        _life = _lifeTime;
    }
}
