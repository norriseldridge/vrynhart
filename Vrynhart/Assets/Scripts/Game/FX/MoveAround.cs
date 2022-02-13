using UnityEngine;

public class MoveAround : MonoBehaviour
{
    [SerializeField]
    float _radius;

    [SerializeField]
    float _speed;

    Vector3 _position;

    // Start is called before the first frame update
    void Start()
    {
        _position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var angle = Time.time * _speed;
        transform.position = new Vector3(_position.x + (_radius * Mathf.Cos(angle)), _position.y + (_radius * Mathf.Sin(angle)));
    }
}
