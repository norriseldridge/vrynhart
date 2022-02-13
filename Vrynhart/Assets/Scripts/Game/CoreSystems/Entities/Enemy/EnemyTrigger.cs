using System.Collections.Generic;
using UnityEngine;
using UniRx;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider2D))]
public class EnemyTrigger : MonoBehaviour
{
    [SerializeField]
    List<EnemyController> _enemies;

    [Header("Audio")]
    [SerializeField]
    AudioClip _triggerSound;

    [SerializeField]
    float _volume = 1;

    bool _triggered = false;

    void Start()
    {
        if (_enemies != null)
        {
            foreach (var e in _enemies)
                e.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered)
            return;

        if (collision.gameObject.GetComponent<PlayerController>())
        {
            _triggered = true;

            if (_triggerSound != null)
                MessageBroker.Default.Publish(new AudioEvent(_triggerSound, _volume));

            foreach (var e in _enemies)
            {
                if (e != null)
                    e.gameObject.SetActive(true);
            }
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
#endif
}
