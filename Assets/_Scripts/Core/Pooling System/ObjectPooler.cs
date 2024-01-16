using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ObjectPooler : SingletonMonoBehaviour<ObjectPooler>
{
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    public void CreatePool(GameObject prefab, PoolTag tag, int size)
    {
        if (poolDictionary.ContainsKey(PoolUtility.TagToString(tag)))
        {
            return;
        }

        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.parent = transform;
            objectPool.Enqueue(obj);
        }

        Pool pool = new Pool(prefab, tag, size);

        poolDictionary.Add(PoolUtility.TagToString(tag), objectPool);

        pools.Add(pool);
    }

    public GameObject DequeueFromPool(PoolTag poolTag)
    {
        string str_tag = PoolUtility.TagToString(poolTag);

        if (!poolDictionary.ContainsKey(str_tag))
        {
            Debug.LogError("Pool with tag " + str_tag + " doesn't excist.");
            return null;
        }

        var queue = poolDictionary[str_tag];
        GameObject go = null;

        if (queue.Count > 0)
        {
            go = queue.Dequeue();
        }
        else
        {
            //Find Prefab and pool
            Pool pool = null;
            foreach (Pool tempPool in pools)
            {
                if (tempPool.tag == poolTag)
                {
                    pool = tempPool;
                }
            }

            go = Instantiate(pool.prefab);
        }

        go.SetActive(false);
        go.transform.parent = null;
        return go;
    }

    public void EnqueueToPool(PoolTag poolTag, GameObject gameObject)
    {
        string tag = PoolUtility.TagToString(poolTag);

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Pool with tag " + tag + " doesn't exist.");
            return;
        }

        //Enqueue transform settings
        gameObject.transform.parent = transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);

        poolDictionary[tag].Enqueue(gameObject);
    }
}
