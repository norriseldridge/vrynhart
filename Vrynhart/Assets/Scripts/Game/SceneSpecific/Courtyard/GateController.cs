using System.Collections;
using UnityEngine;
using UniRx;

public class GateController : MonoBehaviour
{
    [SerializeField]
    string _uid;

    [SerializeField]
    Transform _gate;

    [SerializeField]
    Vector3 _end;

    [SerializeField]
    float _speed;

    [SerializeField]
    Tile _bridgeTile;

    void Start()
    {
        MessageBroker.Default.Receive<GateOpenEvent>()
            .Subscribe(OnGateOpen)
            .AddTo(this);
    }

    void OnGateOpen(GateOpenEvent e)
    {
        StartCoroutine(OpenGateAnimation());
    }

    IEnumerator OpenGateAnimation()
    {
        while (Vector2.Distance(_gate.transform.localPosition, _end) > float.Epsilon)
        {
            _gate.localPosition = Vector2.MoveTowards(_gate.localPosition, _end, _speed * Time.deltaTime);
            yield return null;
        }

        _gate.gameObject.SetActive(false);
        _bridgeTile.enabled = true;

        GameSaveSystem.CacheGame(_uid);
    }
}
