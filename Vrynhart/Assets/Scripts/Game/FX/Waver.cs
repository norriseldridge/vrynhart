using UnityEngine;

public class Waver : MonoBehaviour
{
    [SerializeField]
    Material _material;

    [SerializeField]
    float _speed;

    [SerializeField]
    float _range;

    // Update is called once per frame
    void Update()
    {
        _material.SetVector("_Shear", new Vector2(Mathf.Sin(_speed * Time.time) * _range, 0));
    }
}
