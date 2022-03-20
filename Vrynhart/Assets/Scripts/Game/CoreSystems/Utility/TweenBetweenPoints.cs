using UnityEngine;

public class TweenBetweenPoints : MonoBehaviour
{
    [SerializeField]
    Vector3 _start, _end;

    [SerializeField]
    float _speed;

    bool _movingTowardsEnd;

    void Start()
    {
        transform.localPosition = _start;
        _movingTowardsEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
        var target = _movingTowardsEnd ? _end : _start;
        if (Vector3.Distance(transform.localPosition, target) > float.Epsilon)
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, _speed * Time.deltaTime);
        else
            _movingTowardsEnd = !_movingTowardsEnd;
    }
}
