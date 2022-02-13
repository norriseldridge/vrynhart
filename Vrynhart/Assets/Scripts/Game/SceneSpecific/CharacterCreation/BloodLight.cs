using UnityEngine;

public class BloodLight : MonoBehaviour
{
    [SerializeField]
    float _swing;
    Vector2 _position;

    void Start()
    {
        _position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _position + (Vector2.left * _swing * Mathf.Sin(Time.time));
    }
}
