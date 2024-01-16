using UnityEngine;
using System;

[Serializable]
public class Pool
{
    public PoolTag tag;
    public GameObject prefab;
    public int initialSize;

    public Pool(GameObject prefab, PoolTag tag, int initialSize)
    {
        this.prefab = prefab;
        this.tag = tag;
        this.initialSize = initialSize;
    }
}