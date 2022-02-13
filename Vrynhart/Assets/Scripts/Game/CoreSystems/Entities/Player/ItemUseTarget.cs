using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ItemUseTarget : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _renderer;

    [SerializeField]
    Light2D _light;

    [SerializeField]
    float _speed;

    void Start()
    {
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Set(ItemRecord item, Vector2 position)
    {
        transform.position = position;
        _renderer.sprite = item.Sprite;
        gameObject.SetActive(true);
    }

    void Update()
    {
        _light.pointLightOuterRadius = (1 + 0.5f * Mathf.Sin(Time.time * _speed)) * 0.6f;
    }
}
