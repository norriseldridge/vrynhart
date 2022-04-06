using UnityEngine;

public class BatHead : MonoBehaviour
{
    [SerializeField]
    float _speed;

    [SerializeField]
    float _sway;

    void Update()
    {
        transform.localPosition = _sway * Mathf.Sin(Time.time * _speed) * Vector3.up;
    }
}
