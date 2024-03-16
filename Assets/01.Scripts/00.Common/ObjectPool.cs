using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class ObjectPoolQueue<T> where T : Object
{
    readonly Queue<T> _pool = new Queue<T>();
    public int Count => _pool.Count;

    public void Enqueue(T _o)
    {
        _pool.Enqueue(_o);
    }

    public T GetObject()
    {
        T o;

        if (_pool.Count == 0)
        {
            return null;
        }
        else
        {
            o = _pool.Dequeue();
        }

        return o;
    }

    public void DestroyAll()
    {
        while (_pool.Count > 0)
        {
            var o = _pool.Dequeue();
            Object.Destroy(o);
        }
    }
}