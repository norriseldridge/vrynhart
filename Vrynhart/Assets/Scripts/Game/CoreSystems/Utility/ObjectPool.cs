using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    Queue<T> _pool;

    public ObjectPool(T source, int size)
    {
        _pool = new Queue<T>();

        for (var i = 0; i < size; ++i)
        {
            var spawn = Object.Instantiate(source);
            spawn.gameObject.SetActive(false);
            _pool.Enqueue(spawn);
        }
    }

    public T GetNext()
    {
        var next = _pool.Dequeue();
        _pool.Enqueue(next);
        return next;
    }
}
