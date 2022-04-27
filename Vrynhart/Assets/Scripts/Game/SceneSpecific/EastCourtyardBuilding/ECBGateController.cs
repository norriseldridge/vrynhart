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

    protected override void OnAllLeversActivated(LeverEvent e)
    {
        _tile.enabled = true;

        if (!e.InSaveData) // if we pulled this just now, animate, otherwise just do it because it was in data
            StartCoroutine(TweenGateOut());
        else
            _gate.localPosition += Vector3.down * 2;
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
