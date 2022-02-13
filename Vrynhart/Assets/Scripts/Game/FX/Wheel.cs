using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField]
    float _speed;
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, _speed * Time.time);
    }
}
