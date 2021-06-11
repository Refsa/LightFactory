using System.Collections.Generic;
using UnityEngine;

public abstract class Pooler<T>
{
    Queue<T> pool;

    public Pooler()
    {
        pool = new Queue<T>();
        Expand(16);
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            Expand(16);
        }

        return pool.Dequeue();
    }

    public void Free(T target)
    {
        pool.Enqueue(target);
    }

    void Expand(int by)
    {
        for (int i = 0; i < by; i++)
        {
            pool.Enqueue(Producer());
        }
    }

    protected abstract T Producer();
}