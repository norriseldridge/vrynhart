using System.Collections;
using UnityEngine;
using UniRx;

[ExecuteInEditMode]
public class TileMover : MonoBehaviour
{
    const float Speed = 2.5f;

    [SerializeField]
    bool _playStepSounds;

    public bool ShouldPlayStepSounds => _playStepSounds;
    public bool IsMoving { get; private set; } = false;

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            // snap to grid
            var x = Mathf.RoundToInt(transform.position.x);
            var y = Mathf.RoundToInt(transform.position.y);
            var z = Mathf.RoundToInt(transform.position.z);
            transform.position = new Vector3(x, y, z);
        }
    }
#endif

    public Vector3 GetPositionInDirection(Vector2 direction)
    {
        var x = Mathf.RoundToInt(transform.position.x + direction.x);
        var y = Mathf.RoundToInt(transform.position.y + direction.y);
        var z = Mathf.RoundToInt(transform.position.z);
        return new Vector3(x, y, z);
    }

    public void TryMove(Vector2 direction)
    {
        if (IsMoving)
            return;

        MessageBroker.Default.Publish(new TileMoveEvent(this, direction));
    }

    public void MoveTo(Vector3 destination)
    {
        if (IsMoving)
            return;

        StartCoroutine(TweenTo(destination));
    }

    IEnumerator TweenTo(Vector3 destination)
    {
        IsMoving = true;

        while (Vector2.Distance(transform.position, destination) > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Speed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        IsMoving = false;

        MessageBroker.Default.Publish(new TileMoveCompleteEvent(this));
    }
}