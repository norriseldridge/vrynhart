using System.Collections.Generic;
using UnityEngine;

public class EnableObjectsTrigger : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _objects;

    [SerializeField]
    Transform _target;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
            return;

        if (collision.transform == _target)
        {
            foreach (var go in _objects)
                go.SetActive(true);
        }
    }
}
