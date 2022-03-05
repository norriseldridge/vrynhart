using System.Collections;
using UnityEngine;
using UniRx;

public class SkeletonBossJaw : MonoBehaviour
{
    [SerializeField]
    float _clatterDist;

    [SerializeField]
    float _clatterSpeed;

    [SerializeField]
    float _laughSpeed;

    [SerializeField]
    float _laughDelay;

    [SerializeField]
    float _laughDuration;

    [SerializeField]
    AudioClip _laughSfx;

    Vector3 _position;
    float _nextLaugh;
    float _tick = 0;
    bool _laughing = false;

    // Start is called before the first frame update
    void Start()
    {
        _position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_laughing)
        {
            _nextLaugh -= Time.deltaTime;
            if (_nextLaugh <= 0.0f)
            {
                _nextLaugh = (_laughDelay / 2.0f) + (Random.value * _laughDelay / 0.2f);
                StartCoroutine(Laugh());
            }
        }

        _tick += (_laughing ? _laughSpeed : _clatterSpeed) * Time.deltaTime;
        transform.localPosition = _position + _clatterDist * Mathf.Sin(_tick) * Vector3.up;
    }

    IEnumerator Laugh()
    {
        _laughing = true;
        Brokers.Audio.Publish(new AudioEvent(_laughSfx, 0.7f, position: transform.position));
        yield return new WaitForSeconds(_laughDuration);
        _laughing = false;
    }
}
