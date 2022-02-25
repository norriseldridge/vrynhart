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

        var bounds = _bounds.FirstOrDefault();
        if (bounds != null)
            transform.position = bounds.Limit(_target.position + _offset, _cameraSize);
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