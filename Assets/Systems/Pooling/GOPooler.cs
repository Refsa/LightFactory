using System.Collections.Generic;
using UnityEngine;

public abstract class GOPooler<T> : MonoBehaviour
{
    static GOPooler<T> instance;
    public static GOPooler<T> Instance => instance;

    [SerializeField] GameObject prefab;

    Queue<GameObject> pool;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        
        pool = new Queue<GameObject>();
        Expand(16);
    }

    void Expand(int by)
    {
        for (int i = 0; i < by; i++)
        {
            var go = GameObject.Instantiate(prefab);
            go.transform.SetParent(transform);
            Free(go);
        }
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            Expand(16);
        }

        Debug.Log("Get");

        var go = pool.Dequeue();
        go.SetActive(true);
        return go;
    }

    public void Free(GameObject go)
    {
        go.SetActive(false);
        pool.Enqueue(go);
    }
}