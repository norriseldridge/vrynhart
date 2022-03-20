using System.Collections;
using UnityEngine;

public class ECBGateController : LeverResponder
{
    [SerializeField]
    Tile _tile;

    [SerializeField]
    Transform _gate;

    [SerializeField]
    AudioClip _clip;

    protected override void OnAllLeversActivated()
    {
        _tile.enabled = true;
        StartCoroutine(TweenGateOut());
    }

    IEnumerator TweenGateOut()
    {
        Brokers.Audio.Publish(new AudioEvent(_clip));
        var speed = 8;
        for (var i = 0; i < speed * 2; ++i)
        {
            _gate.localPosition += speed * Time.deltaTime * Vector3.down;
            yield return null;
        }
    }
}
