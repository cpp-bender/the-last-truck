using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Stage Data", fileName = "Stage Data")]
public class StageData : ScriptableObject
{
    [Header("STAGE END BONUS MONEY")]
    [SerializeField] float stageEndBonusMoney;
    [Header("ENEMY SPAWN SETTINGS"), Space(5f)]
    [SerializeField] int enemySpawnWaveCount;
    [SerializeField] float largeZombieProbability;
    [SerializeField] float mediumZombieProbability;
    [SerializeField] int zombieCountFactor;
    [SerializeField] float initialPosZ;
    [SerializeField] float distBetween;

    [SerializeField] SideSpawnPointData sideSpawnPointData;
    public float LargeZombieProbability { get => largeZombieProbability; }
    public float MediumZombieProbability { get => mediumZombieProbability; }
    public int ZombieCountFactor { get => zombieCountFactor; }
    public float EnemySpawnWaveCount { get => enemySpawnWaveCount; }
    public float DistBetween { get => distBetween; }
    public float InitialPosZ { get => initialPosZ; }
    public SideSpawnPointData SideSpawnPointData { get => sideSpawnPointData; }
    public float StageEndBonusMoney { get => stageEndBonusMoney; }
}
