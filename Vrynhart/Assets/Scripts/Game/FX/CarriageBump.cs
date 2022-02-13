using System.Collections;
using UnityEngine;

public class CarriageBump : MonoBehaviour
{
    [SerializeField]
    float _frequency;
    [SerializeField]
    float _amount;
    float _nextTime;

    void Start()
    {
        _nextTime = Random.Range(0, _frequency);
    }

    // Update is called once per frame
    void Update()
    {
        _nextTime -= Time.deltaTime;
        if (_nextTime <= 0)
        {
            _nextTime = Random.Range(0, _frequency);
            StartCoroutine(Bump());
        }
    }

    IEnumerator Bump()
    {
        transform.localPosition = Vector3.up * _amount;
        yield return null;
        transform.localPosition = Vector3.zero;
    }
}
