using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DemonicEyeController : MonoBehaviour
{
    [SerializeField]
    Transform _target;

    [SerializeField]
    float _closeRange;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    SpriteMask _mask;

    [SerializeField]
    Light2D _light;

    [SerializeField]
    Transform _iris;

    [SerializeField]
    float _irisRange;

    bool _closed = false;

    public void Close() => _closed = true;

    // Update is called once per frame
    void Update()
    {
        _mask.sprite = _renderer.sprite;

        if (_target != null)
        {
            var maxRange = _irisRange * 2;
            _iris.localPosition = new Vector3(Mathf.Clamp(Mathf.Lerp(0, _target.position.x - transform.position.x, _irisRange), -maxRange, maxRange), 0, 0);

            var dist = Vector2.Distance(transform.position, _target.position);
            if (dist <= _closeRange)
                _closed = true;
        }

        _iris.localScale = (1f + 0.05f * Mathf.Sin(Time.time * 1.2f)) * Vector3.one;

        if (_closed)
        {
            _animator.Play("DemonicEye");
            StartCoroutine(FadeLight());
        }
    }

    IEnumerator FadeLight()
    {
        while (_light.intensity > 0f)
        {
            _light.intensity -= 0.01f * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
