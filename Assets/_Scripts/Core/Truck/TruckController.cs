using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TruckController : KillableObject, IStageController, IGameController, IGameFail
{
    #region Singleton
    public static TruckController Instance { get { return instance; } }
    private static TruckController instance;
    public bool isUnstoppable;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    [Header("DEPENDENCIES")]
    public List<GameObject> deckParts;
    public GameObject backCollider;
    public MeshRenderer healthBar;
    public List<GameObject> explosionParts;
    public List<GunSlot> gunSlots;

    [Header("DEBUG")]
    public int slotIndex;
    public int durability;
    public float income;

    private const float BaseIncome = 0.5f;
    private SaveManager saveManager;
    private Material healthBarMat;

    private bool canShake;
    public Vector3 levelStartPos;

    private void Start()
    {
        levelStartPos = transform.position;

        saveManager = SaveManager.Instance;

        healthBarMat = healthBar.material;

        LoadTruckData();

        currentHealth = MaxHealth();
    }

    private void LoadTruckData()
    {
        var saveManager = SaveManager.Instance;

        var truckData = saveManager.LoadTruckData();

        slotIndex = truckData.deckIndex;
        HandleSlotIndex();

        durability = truckData.durability;
        income = truckData.income;
    }

    private void HandleWeaponDeck()
    {
        for (int i = 0; i < deckParts.Count; i++)
        {
            if (slotIndex >= i)
            {
                deckParts[i].gameObject.SetActive(true);
            }
            else
            {
                deckParts[i].gameObject.SetActive(false);
            }
        }

        UpdateBackCollider();

        void UpdateBackCollider()
        {
            backCollider.transform.localPosition = new Vector3(backCollider.transform.localPosition.x, backCollider.transform.localPosition.y, -1f + -slotIndex);
        }
    }

    private void HandleSlotIndex()
    {
        if (slotIndex == 0)
        {
            for (int i = 0; i < gunSlots.Count; i++)
            {
                if (i < 3)
                {
                    gunSlots[i].Unlock();
                }
                else
                {
                    gunSlots[i].Lock();
                }
            }
        }

        else if (slotIndex == 1)
        {
            for (int i = 0; i < gunSlots.Count; i++)
            {
                if (i < 5)
                {
                    gunSlots[i].Unlock();
                }
                else
                {
                    gunSlots[i].Lock();
                }
            }
        }
        else if (slotIndex == 2)
        {
            for (int i = 0; i < gunSlots.Count; i++)
            {
                gunSlots[i].Unlock();
            }
        }
    }

    public void UpdateDurability()
    {
        durability += (int)(durability * 0.05);

        saveManager.SaveTruckData(slotIndex, durability, income);
    }

    public void UpdateWeaponDeck()
    {
        slotIndex++;

        saveManager.SaveTruckData(slotIndex, durability, income);

        HandleSlotIndex();
    }

    public void UpdateIncome()
    {
        income += 0.1f * GameManager.instance.levelSaveData.inflationRate;

        saveManager.SaveTruckData(slotIndex, durability, income);
    }

    public LevelSaveData CalculateInflationRate()
    {
        LevelSaveData levelSaveData = new LevelSaveData();

        levelSaveData.inflationRate = income / BaseIncome;

        return levelSaveData;
    }

    public void OnStageStart()
    {
        currentHealth = MaxHealth();
        FillHealthBar(currentHealth / MaxHealth());

        LoadTruckData();

        canShake = true;
        StartCoroutine(ShakeRoutine());
    }

    public void OnStageEnd()
    {
        currentHealth = MaxHealth();
        FillHealthBar(currentHealth / MaxHealth());

        canShake = false;
        StopCoroutine(ShakeRoutine());
        StabilizeTruck();
    }

    public void ExplodeTruck()
    {
        EffectManager.Instance.ShowEffect(transform.position, PoolTag.TruckExplosion);

        for (int i = 0; i < explosionParts.Count; i++)
        {
            Rigidbody rb = explosionParts[i].AddComponent<Rigidbody>();
            MeshCollider meshCollider = explosionParts[i].GetComponent<MeshCollider>();
            meshCollider.enabled = true;
            rb.AddExplosionForce(500f, transform.position, 2f, 20f);
        }

        for (int i = 0; i < explosionParts.Count; i++)
        {
            explosionParts[i].transform.SetParent(null);
        }

        gameObject.SetActive(false);
        PlayerController.Instance.gameObject.SetActive(false);

        GameManager.instance.LevelFail();

        PlatformController.Instance.state = PlatformState.Stop;
    }

    public override void TakeDamage(float damage = 1)
    {
        base.TakeDamage(damage);

        FillHealthBar(currentHealth / MaxHealth());
    }
    private IEnumerator ShakeRoutine()
    {
        while (canShake)
        {
            float rnd = Random.Range(10f, 15f);

            yield return new WaitForSeconds(rnd);

            Shake();
        }
    }

    public void Shake()
    {
        Sequence shakeSeq = DOTween.Sequence();

        Tween leftRotate = transform.DORotate(new Vector3(0f, 180f, -5f), .2f);

        Tween rightRotate = transform.DORotate(new Vector3(0f, 180f, 5f), .2f);

        Tween midRotate = transform.DORotate(new Vector3(0f, 180f, 0f), .2f);

        shakeSeq.Append(rightRotate).Append(leftRotate).Append(midRotate);

        shakeSeq.Play();
    }

    public void StabilizeTruck()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
    }

    private void FillHealthBar(float fillAmount)
    {
        healthBarMat.SetFloat("_Health", fillAmount);
    }

    protected override void DamageEffect()
    {

    }

    protected override void Death()
    {
        if (isUnstoppable)
            return;

        isDead = true;
        ExplodeTruck();
        TruckDeadState.Instance.TruckDead();
    }
    private float MaxHealth()
    {
        return durability;
    }

    protected override void EarnMoney()
    {
        ;
    }

    protected override void ScaleDown()
    {

    }

    protected override void Suicide()
    {
        ;
    }

    public void OnLevelStart()
    {
        canShake = true;
        StartCoroutine(ShakeRoutine());
    }

    public void OnLevelEnd()
    {
        canShake = false;
        StopCoroutine(ShakeRoutine());
        StabilizeTruck();

        saveManager.SaveLevelData(CalculateInflationRate());
    }

    public void OnGameFail()
    {
        saveManager.SaveTruckData(slotIndex, durability, income);
    }
}
