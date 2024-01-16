using System.Collections;
using UnityEngine;

public class SideSpawnPoint : MonoBehaviour, IStageController, IGameController
{
    [Header("DEPENDENCIES")]
    public Vector3 lookDir;

    private int zombieCount;
    private int count;
    private float interval;

    private void Start()
    {
        transform.SetParent(null);
    }

    public void SetSpawnPointData(SideSpawnPointData sideSpawnPointData)
    {
        zombieCount = sideSpawnPointData.zombieCount;
        count = sideSpawnPointData.waveCount;
        interval = sideSpawnPointData.waveInterval;
    }

    public IEnumerator ZombieSpawnRoutine()
    {
        while (count > 0)
        {
            yield return new WaitForSeconds(interval);

            int rndValue = Random.Range(zombieCount - 1, zombieCount + 1);

            for (int i = 0; i < rndValue; i++)
            {
                GameObject zombie = ObjectPooler.Instance.DequeueFromPool(PoolTag.ZombieSmall);

                zombie.gameObject.SetActive(true);

                zombie.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

                Vector3 rndPos = new Vector3(Random.insideUnitSphere.x * 3f, transform.position.y, Random.insideUnitSphere.z * 3f);

                zombie.transform.position = transform.position + rndPos;
                
                zombie.GetComponent<Zombie>().skinnedMeshRenderer.material =
                    LevelManager.Instance.levelColorData.ZombieColors[(GameManager.instance.currentLevel - 1) % 5];
            }

            count--;
        }
    }

    public void OnLevelStart()
    {
        var truckDeadState = FindObjectOfType<TruckDeadState>();

        if (!truckDeadState.IsTruckDead())
        {
            StartCoroutine(ZombieSpawnRoutine());
        }
    }

    public void OnLevelEnd()
    {
        StopCoroutine(ZombieSpawnRoutine());
    }

    public void OnStageStart()
    {
        StartCoroutine(ZombieSpawnRoutine());
    }

    public void OnStageEnd()
    {
        StopCoroutine(ZombieSpawnRoutine());
    }
}
