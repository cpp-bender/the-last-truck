using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Bullet Data", fileName = "Bullet Data")]
public class BulletData : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] PoolTag tag = PoolTag.CanonBullet;

    public PoolTag Tag { get => tag; set => tag = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
}
