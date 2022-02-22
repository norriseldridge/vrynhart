using UnityEngine;
using UniRx;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Transform _target;

    CameraBounds _bounds;
    Vector2 _cameraSize;
    Vector3 _offset = new Vector3(0, 0, -10);

    void Awake()
    {
        MessageBroker.Default.Receive<CameraBoundsChangeEvent>()
            .Subscribe(OnCameraBoundsChanged)
            .AddTo(this);
    }

    void Start()
    {
        var verticalSize = _camera.orthographicSize * 2.0f;
        var horizontalSize = verticalSize * Screen.width / Screen.height;
        _cameraSize = new Vector2(horizontalSize, verticalSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null) return;

        if (_bounds != null)
            transform.position = _bounds.Limit(_target.position + _offset, _cameraSize);
        else
            transform.position = _target.position + _offset;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _cameraSize);
    }
#endif

    void OnCameraBoundsChanged(CameraBoundsChangeEvent e)
    {
        if (e.Entering)
            _bounds = e.Bounds;
        else
        {
            if (_bounds == e.Bounds)
                _bounds = null;
        }
    }
}