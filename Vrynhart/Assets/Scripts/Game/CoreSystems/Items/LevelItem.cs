using UnityEngine;
using UniRx;

[ExecuteInEditMode]
[RequireComponent(typeof(CircleCollider2D))]
public class LevelItem : MonoBehaviour
{
    [SerializeField]
    string _itemId;

    [SerializeField]
    int _count = 1;

    [SerializeField]
    [Range(0, 1)]
    float _appearancePercent = 1;

    void Start()
    {
        if (Application.isPlaying)
        {
            gameObject.SetActive(Random.value <= _appearancePercent);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            MessageBroker.Default.Publish(new ItemPickUpEvent(_itemId, _count));
            Destroy(gameObject);
        }
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
        Gizmos.color = new Color(1, 1, 0, 0.25f);
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
#endif
}
