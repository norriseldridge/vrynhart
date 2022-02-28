using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GiantSlugTentacle : MonoBehaviour
{
    [SerializeField]
    EnemyController _enemyController;

    [SerializeField]
    List<Transform> _swrils;

    [SerializeField]
    float _swirlInTime;

    [SerializeField]
    float _inTime;

    [SerializeField]
    float _outTime;

    [SerializeField]
    float _upTime;

    [SerializeField]
    Transform _view;

    [SerializeField]
    AudioClip _warningSfx;

    [SerializeField]
    float _warningVolume;

    void Start()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        _enemyController.enabled = false;
        MessageBroker.Default.Publish(new AudioEvent(_warningSfx, _warningVolume, priority: 1));
        yield return TweenInSwirls();

        var task = MessageBroker.Default.Receive<TurnProgressionEvent>().Take(2).ToTask();
        yield return new WaitUntil(() => task.IsCompleted);

        _enemyController.enabled = true;
        MessageBroker.Default.Publish(new TileMoveCompleteEvent(_enemyController.TileMover));

        yield return TweenInAndOut();
        _enemyController.enabled = false;
        yield return TweenOutSwirls();
        Destroy(gameObject);
    }

    IEnumerator TweenInSwirls()
    {
        var targetSizes = new List<Vector3>();
        foreach (var swirl in _swrils)
            targetSizes.Add(swirl.localScale);

        for (var i = 0.0f; i < _swirlInTime; i += Time.deltaTime)
        {
            for (int index = 0; index < _swrils.Count; ++index)
                _swrils[index].localScale = targetSizes[index] * (i / _swirlInTime);
            yield return null;
        }
    }

    IEnumerator TweenInAndOut()
    {
        var pos = new Vector3(0, -4, 0);
        for (var i = 0.0f; i < _inTime; i += Time.deltaTime)
        {
            pos.y = -4 + (4.0f * i / _inTime);
            _view.localPosition = pos;
            yield return null;
        }

        yield return new WaitForSeconds(_upTime);

        pos = new Vector3(0, 0, 0);
        for (var i = 0.0f; i < _outTime; i += Time.deltaTime)
        {
            pos.y = -4.0f * i / _outTime;
            _view.localPosition = pos;
            yield return null;
        }
    }

    IEnumerator TweenOutSwirls()
    {
        var targetSizes = new List<Vector3>();
        foreach (var swirl in _swrils)
            targetSizes.Add(swirl.localScale);

        for (var i = 0.0f; i < _swirlInTime; i += Time.deltaTime)
        {
            for (int index = 0; index < _swrils.Count; ++index)
                _swrils[index].localScale = targetSizes[index] * ((_swirlInTime - i) / _swirlInTime);
            yield return null;
        }
    }
}
