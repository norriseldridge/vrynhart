using UnityEngine;

public enum EnemyVisualState
{
    Idle,
    Run
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(TakeDamageView))]
public class EnemyView : MonoBehaviour
{
    SpriteRenderer _renderer;
    Animator _animator;
    TakeDamageView _takeDamageView;
    EnemyVisualState _state = EnemyVisualState.Idle;

    public bool Flipped => _renderer.flipX;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _takeDamageView = GetComponent<TakeDamageView>();
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

    public void TakeHit() => _takeDamageView.TakeHit();
}
