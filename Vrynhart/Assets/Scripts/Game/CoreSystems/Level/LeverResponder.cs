using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class LeverResponder : MonoBehaviour
{
    [SerializeField]
    List<string> _leverIds;

    int _leversActive = 0;

    void Start()
    {
        Brokers.Default.Receive<LeverEvent>()
            .Where(e => _leverIds.Contains(e.LeverID))
            .Subscribe(OnLeverEvent)
            .AddTo(this);
    }

    void OnLeverEvent(LeverEvent e)
    {
        _leversActive++;

        if (_leversActive == _leverIds.Count)
            OnAllLeversActivated(e);
    }

    protected virtual void OnAllLeversActivated(LeverEvent e) { }
}
