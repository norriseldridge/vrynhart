using System.Collections;
using UnityEngine;

public enum EnemyVisualState
{
    Idle,
    Run
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EnemyView : MonoBehaviour
{
    SpriteRenderer _renderer;
    Animator _animator;
    EnemyVisualState _state = EnemyVisualState.Idle;

    public bool Flipped => _renderer.flipX;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public void FaceTowards(Vector2 target)
    {
        if (target.x != transform.position.x)
            _renderer.flipX = target.x < transform.position.x;
    }

    public void SetState(EnemyVisualState state)
    {
        if (_state != state)
        {
            _state = state;

            var stateName = _state.ToString();
            _animator.Play(stateName);
        }
    }

    public void TakeHit()
    {
        StartCoroutine(FlashColor(Color.red));
    }

    IEnumerator FlashColor(Color target)
    {
        var color = _renderer.color;
        for (var i = 0; i < 3; ++i)
        {
            _renderer.color = target;
            yield return new WaitForSeconds(0.1f);
            _renderer.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        _renderer.color = color;
    }
}
