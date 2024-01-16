using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Enemy Spawn Data", fileName = "Enemy Spawn Data")]
public class EnemySpawnData : ScriptableObject
{
    [SerializeField] private int count;

    public int Count { get => count; }
}
