using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    [Header("DEPENDENCIES"), Space(5f)]
    public List<LevelData> levels;
    public GameObject[] zombiePrefabs;

    [Header("DEBUG"), Space(5f)]
    public LevelData currentLevel;
    public StageData currentStage;

    public PlatformController currentPlatform;
    public Transform levelEndTransform;
    public LevelColorData levelColorData;

    protected override void Awake()
    {
        base.Awake();
        InitLevel();
    }

    public void InitLevel()
    {
        currentLevel = GameManager.instance.currentLevel < 10 ? GetEasyLevel() : GetHardLevel();

        currentStage = currentLevel.Stages[GameManager.instance.currentStage - 1];

        InitPlatform();

        InitEnemies();

        InitSideSpawnPoints();

        UIManager.instance.SetSuccessText("Challange " + GameManager.instance.currentLevel + " Completed");
    }

    private LevelData GetEasyLevel()
    {
        return levels[(GameManager.instance.currentLevel - 1) % levels.Count];
    }

    private LevelData GetHardLevel(int startIndex = 3, int endIndex = 10)
    {
        int index = Random.Range(startIndex, endIndex);

        return levels[index];
    }

    private void InitPlatform()
    {
        GameObject platform = Instantiate(currentLevel.PlatformData.platformPrefab, currentLevel.PlatformData.pos, Quaternion.identity);
        platform.name = "PLATFORM";
        currentPlatform = platform.GetComponent<PlatformController>();

        currentPlatform.RandomizeColors();

        levelEndTransform = InitLevelEnd();

        Transform InitLevelEnd()
        {
            GameObject levelEnd = Instantiate(currentLevel.PlatformData.levelEndPrefab, currentPlatform.transform);
            var levelEndScript = levelEnd.GetComponent<LevelEnd>();
            levelEndScript.levelEndTrigger.SwitchRendererTo(false);

            var posZ = currentStage.InitialPosZ + (currentStage.EnemySpawnWaveCount * currentStage.DistBetween) + 22f;
            levelEnd.transform.localPosition = new Vector3(0f, 1f, posZ);

            return levelEndScript.levelEndTrigger.transform;
        }
    }

    private void InitEnemies()
    {
        var randomVector3s = new List<Vector3>();

        float z = currentStage.InitialPosZ;

        ObjectPooler.Instance.CreatePool(zombiePrefabs[0], PoolTag.ZombieSmall, 5);
        ObjectPooler.Instance.CreatePool(zombiePrefabs[1], PoolTag.ZombieMedium, 5);
        ObjectPooler.Instance.CreatePool(zombiePrefabs[2], PoolTag.ZombieLarge, 5);

        for (int i = 0; i < currentStage.EnemySpawnWaveCount; i++)
        {
            int enemyCountForThisWave =
                Random.Range(1 + currentStage.ZombieCountFactor, 5 + currentStage.ZombieCountFactor);
            
            for (int j = 0; j < enemyCountForThisWave; j++)
            {
                float x = Random.Range(-5f, 5f);

                float zombieSizeProbability = Random.value;

                GameObject zombie;

                if (zombieSizeProbability <= currentStage.LargeZombieProbability)
                {
                    zombie = ObjectPooler.Instance.DequeueFromPool(PoolTag.ZombieLarge);
                }
                else if (zombieSizeProbability <= currentStage.MediumZombieProbability)
                {
                    zombie = ObjectPooler.Instance.DequeueFromPool(PoolTag.ZombieMedium);
                }
                else
                {
                    zombie = ObjectPooler.Instance.DequeueFromPool(PoolTag.ZombieSmall);
                }

                zombie.GetComponent<Zombie>().skinnedMeshRenderer.material =
                    levelColorData.ZombieColors[(GameManager.instance.currentLevel - 1) % 5];

                zombie.gameObject.SetActive(true);

                zombie.transform.position = GetRandomVector3();

                zombie.transform.SetParent(currentPlatform.transform);

                zombie.transform.localPosition = new Vector3(zombie.transform.localPosition.x + x, 0.333f, zombie.transform.localPosition.z + z);

            }
            z += currentStage.DistBetween;
        }

        Vector3 GetRandomVector3()
        {
            float r = 2f;

            var rndVector3 = new Vector3(Random.insideUnitSphere.x * r * 10f, 0f, Random.insideUnitSphere.z * r);

            if (randomVector3s.Contains(rndVector3))
            {
                GetRandomVector3();
            }

            randomVector3s.Add(rndVector3);

            return rndVector3;
        }
    }

    private void InitSideSpawnPoints()
    {
        currentPlatform.sideSpawnPointRight.SetSpawnPointData(currentStage.SideSpawnPointData);

        currentPlatform.sideSpawnPointLeft.SetSpawnPointData(currentStage.SideSpawnPointData);
    }

    private void InitNextStage()
    {
        currentStage = currentLevel.Stages[GameManager.instance.currentStage - 1];

        currentPlatform.transform.position = currentLevel.PlatformData.pos;

        InitEnemies();

        InitSideSpawnPoints();

        OnNewStageStart();
    }

    private void OnNewStageStart()
    {
        var stageControllers = HyperUtilities.FindInterfacesOfType<IStageController>();

        foreach (var stageController in stageControllers)
        {
            stageController.OnStageStart();
        }
    }

    private void OnNewStageEnd()
    {
        GainStageEndBonusMoney();

        var stageControllers = HyperUtilities.FindInterfacesOfType<IStageController>();

        foreach (var stageController in stageControllers)
        {
            stageController.OnStageEnd();
        }
    }

    public void GainStageEndBonusMoney()
    {
        EffectManager.Instance.ShowGainEffect(levelEndTransform.position + Vector3.forward * 5f, Mathf.RoundToInt(currentStage.StageEndBonusMoney * GameManager.instance.levelSaveData.inflationRate), 3f, 2f);
        PlayerController.Instance.GainMoney(Mathf.RoundToInt(currentStage.StageEndBonusMoney * GameManager.instance.levelSaveData.inflationRate));
    }

    #region StageProgressBar
    public int GetLevelStageCount()
    {
        return currentLevel.Stages.Count;
    }

    public int GetCurrentStageCount()
    {
        return currentLevel.Stages.IndexOf(currentStage) + 1;
    }

    public Vector3 StartPosition()
    {
        return Vector3.zero;
    }

    public Vector3 EndPosition()
    {

        return Vector3.zero;
    }
    #endregion

    public void MoveNextPhase()
    {
        OnNewStageEnd();
    }

    public void InitNextPhase()
    {
        if (HasStage())
        {
            InitNextStage();
        }
    }

    public bool HasStage()
    {
        return true ? (GameManager.instance.currentStage - 1) < currentLevel.Stages.Count : false;
    }
}
