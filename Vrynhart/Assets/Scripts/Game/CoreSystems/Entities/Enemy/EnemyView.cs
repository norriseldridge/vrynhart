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
}
