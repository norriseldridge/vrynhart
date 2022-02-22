using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField]
    Transform _arm;

    [SerializeField]
    float _swing;

    [SerializeField]
    float _speed;

    void Update()
    {
        _arm.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * _speed) * _swing);
    }
}
