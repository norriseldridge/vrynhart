using UnityEngine;
using UniRx;

[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    [SerializeField]
    bool _isFloor;

    [Header("Audio")]
    [SerializeField]
    AudioClip _step;

    [SerializeField]
    float _volume = 0.15f;

    [SerializeField]
    float _minPitch = 1;

    [SerializeField]
    float _maxPitch = 1;

    public bool IsFloor => _isFloor;

    public void PlayStepSound()
    {
        MessageBroker.Default.Publish(new AudioEvent(_step, _volume, _minPitch, _maxPitch, transform.position));
    }

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

    void OnDrawGizmosSelected()
    {
        // highlight self
        Gizmos.color = new Color(_isFloor ? 0 : 1, _isFloor ? 1 : 0, 0, 0.25f);
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
#endif
}
