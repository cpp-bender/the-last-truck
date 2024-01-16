using System.Collections;
using UnityEngine;

public class PlatformController : SingletonMonoBehaviour<PlatformController>, IGameController
{
    [Header("DEPENDENCIES")]
    public PlatformMovementData platformMovementData;
    public SideSpawnPoint sideSpawnPointRight;
    public SideSpawnPoint sideSpawnPointLeft;
    public LevelColorData levelColors;
    public MeshRenderer[] mainRenderers;
    public MeshRenderer[] nearRenderers;
    public MeshRenderer[] sideRenderers;

    [Header("DEBUG")]
    public PlatformState state = PlatformState.Stop;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        Move();
    }

    public IEnumerator CheckPlatformEdges()
    {
        while (true)
        {
            if (transform.position.z <= -300f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -250f);
            }

            yield return null;
        }
    }

    private void Move()
    {
        if (state == PlatformState.Move)
        {
            transform.Translate(platformMovementData.MoveDir * platformMovementData.MoveSpeed * Time.deltaTime);
        }
    }

    public void OnLevelStart()
    {
        if (TruckDeadState.Instance.IsTruckDead())
        {
            state = PlatformState.Stop;
        }
        else
        {
            state = PlatformState.Move;
        }
    }

    public void OnLevelEnd()
    {
        state = PlatformState.Stop;
    }

    public void MoveThePlatform()
    {
        state = PlatformState.Move;
    }

    public void StopThePlatform()
    {
        state = PlatformState.Stop;
    }

    public void RandomizeColors()
    {
        RandomizeMainRenderers();

        RandomizeNearRenderers();

        RandomizeSideRenderers();

        void RandomizeMainRenderers()
        {
            int minRange = GameManager.instance.currentLevel - 1;

            int maxRange = levelColors.MainColors.Length;

            foreach (var mainRenderer in mainRenderers)
            {
                mainRenderer.material = levelColors.MainColors[minRange % maxRange];
            }
        }

        void RandomizeNearRenderers()
        {
            int minRange = GameManager.instance.currentLevel - 1;

            int maxRange = levelColors.NearColors.Length;

            foreach (var nearRenderer in nearRenderers)
            {
                nearRenderer.material = levelColors.NearColors[minRange % maxRange];
            }
        }

        void RandomizeSideRenderers()
        {
            int minRange = GameManager.instance.currentLevel - 1;

            int maxRange = levelColors.NearColors.Length;

            foreach (var sideRenderer in sideRenderers)
            {
                sideRenderer.material = levelColors.SideColors[minRange % maxRange];
            }
        }
    }
}