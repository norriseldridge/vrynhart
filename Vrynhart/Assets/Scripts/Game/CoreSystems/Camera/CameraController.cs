using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Transform _target;

    List<CameraBounds> _bounds = new List<CameraBounds>();
    Vector2 _cameraSize;
    Vector3 _offset = new Vector3(0, 0, 0);
    float _originalCameraSize;

    IReactiveProperty<CameraBounds> _currentBounds = new ReactiveProperty<CameraBounds>();

    void Awake()
    {
        MessageBroker.Default.Receive<CameraBoundsChangeEvent>()
            .Subscribe(OnCameraBoundsChanged)
            .AddTo(this);

        _currentBounds.Subscribe(OnBoundsChange).AddTo(this);
    }

    void Start()
    {
        _originalCameraSize = _camera.orthographicSize;
        UpdateCameraSize();
    }

    void OnBoundsChange(CameraBounds bounds)
    {
        if (bounds == null)
            return;

        if (bounds.HasCameraSizeOverride)
        {
            _camera.orthographicSize = bounds.CameraSizeOverride;
        }
        else
        {
            _camera.orthographicSize = _originalCameraSize;
        }

        UpdateCameraSize();
    }

    void UpdateCameraSize()
    {
        var verticalSize = _camera.orthographicSize * 2.0f;
        var horizontalSize = verticalSize * Screen.width / Screen.height;
        _cameraSize = new Vector2(horizontalSize, verticalSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null) return;

        _currentBounds.Value = _bounds.LastOrDefault();
        if (_currentBounds.Value != null)
        {
            var offset = _currentBounds.Value.HasOffsetOverride ? _currentBounds.Value.transform.position + _currentBounds.Value.OffsetOverride : _target.position + _offset;
            transform.position = _currentBounds.Value.Limit(offset, _cameraSize);
        }
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
            _bounds.Add(e.Bounds);
        else
        {
            _bounds.Remove(e.Bounds);
        }
    }
}