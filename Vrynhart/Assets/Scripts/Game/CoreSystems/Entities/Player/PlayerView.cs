using System.Collections;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

public enum PlayerState
{
    Idle,
    Run
}

public enum PlayerFacing
{
    Left,
    Right
}

public class PlayerView : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField]
    Animator _hat;

    [SerializeField]
    Animator _clothes;

    [Header("Parts")]
    [SerializeField]
    SpriteRenderer _head;

    [SerializeField]
    SpriteRenderer _hair;

    [SerializeField]
    SpriteRenderer _eyes;

    Animator[] _animators;
    PlayerState _state = PlayerState.Idle;
    PlayerFacing _facing = PlayerFacing.Right;

    List<Material> _materials;

    void Awake()
    {
        Brokers.Default.Receive<PlayerViewDataEvent>()
            .Subscribe(OnViewDataChanged)
            .AddTo(this);

        Brokers.Default.Receive<HealthChangeEvent>()
            .Where(e => e.Change < 0)
            .Subscribe(OnTakeDamage)
            .AddTo(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _animators = GetComponentsInChildren<Animator>();

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = new List<Material>();
        foreach (var renderer in renderers)
            _materials.Add(renderer.material);
    }

    void OnViewDataChanged(PlayerViewDataEvent viewDataEvent) => UpdateView(viewDataEvent.ViewData);

    void UpdateView(PlayerViewData viewData)
    {
        _head.color = viewData.Head;
        _hair.color = viewData.Hair;
        _eyes.color = viewData.Eyes;

        var hat = ItemsLookup.GetItem(viewData.HatId);
        _hat.runtimeAnimatorController = hat.ClothingAnimator;

        var clothes = ItemsLookup.GetItem(viewData.ClothesId);
        _clothes.runtimeAnimatorController = clothes.ClothingAnimator;

        if (_animators != null)
        {
            foreach (var ani in _animators)
                ani.Play(_state.ToString(), -1, 0);
        }
    }

    void OnTakeDamage(HealthChangeEvent e)
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        var orig = transform.localPosition;
        var off = orig;
        for (int i = 0; i < 16; ++i)
        {
            off.x = (Mathf.Sin(i * 0.25f) - 0.5f) * 0.1f;
            transform.localPosition = off;
            yield return null;
        }
        transform.localPosition = orig;
    }

    public void SetState(PlayerState state)
    {
        if (_state != state)
        {
            _state = state;

            var stateName = _state.ToString();
            foreach (var ani in _animators)
                ani.Play(stateName);
        }
    }

    public void SetDirection(PlayerFacing facing)
    {
        if (_facing != facing)
        {
            _facing = facing;

            var flip = _facing != PlayerFacing.Right;
            transform.localScale = new Vector3(flip ? -1 : 1, 1);
        }
    }

    public void SetIFrameState(bool hasIFrames)
    {
        foreach (var mat in _materials)
            mat.SetColor("_Tint", hasIFrames ? Color.red : Color.white);
    }
}
