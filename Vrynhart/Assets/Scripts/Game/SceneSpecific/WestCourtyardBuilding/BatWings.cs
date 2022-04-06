using UnityEngine;

public class BatWings : MonoBehaviour
{
    [SerializeField]
    float _speed;

    [SerializeField]
    float _scaleAmount;

    void Update()
    {
        var scale = 1 + (_scaleAmount * (1 + Mathf.Sin(Time.time * _speed)) * 0.5f);
        transform.localScale = new Vector3(scale, 1, 1);
    }
}
