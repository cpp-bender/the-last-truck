using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunSlot : MonoBehaviour, IStageController, IGameFail
{
    [Header("Level Up Effect Position")]
    [SerializeField] float yPos;
    [SerializeField] float xPos;

    [Header("DEPENDENCIES")]
    [SerializeField] List<Turret> turrets;
    [SerializeField] UpgradeImage imageUpgrader;
    public TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject lockArea;
    [SerializeField] GameObject smokeFX;

    [Header("DEBUG")]
    public TurretSaveData turretSaveData;
    public int slotIndex = 0;

    public Turret currentTurret;
    private MeshRenderer meshRenderer;
    private bool isMoneyFlowActive = false;
    private bool isClosed = false;

    private float moneyInvestRate = 0.033f;
    private float moneyInvestDuration = 2f;
    private float lastMoneyInvestTime = 0f;
    private int investedMoney = 0;
    private int totalLevelCount = 0;
    private int neededMoney;

    //TUTORIAL
    public bool firstCanonUnlocked;
    public bool firstCannonUpgraded;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        // LoadTurretSaveData();
    }

    #region Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            imageUpgrader.Fill();

            currentTurret.Buff();

            SwitchRendererTo(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            imageUpgrader.UnFill();

            currentTurret.DeBuff();

            SwitchRendererTo(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isMoneyFlowActive && !isClosed)
        {
            if (CanMoneyInvest())
            {
                investedMoney += PlayerController.Instance.LoseMoney(CalculateLosePrice());

                SetPriceText();

                if (CanLevelUp())
                {
                    if (LevelManager.Instance.currentLevel.TutorialLevel && !firstCanonUnlocked && slotIndex == 0)
                    {
                        firstCanonUnlocked = true;
                    }

                    else if (LevelManager.Instance.currentLevel.TutorialLevel && !firstCannonUpgraded && slotIndex == 0)
                    {
                        firstCannonUpgraded = true;
                    }

                    LevelUp();
                }

                lastMoneyInvestTime = Time.time;
            }
        }
    }

    private void SwitchRendererTo(bool turnOn)
    {
        meshRenderer.enabled = turnOn;
    }

    public void ActiveMoneyFlow()
    {
        isMoneyFlowActive = true;

        CalculateMoneyInvestRate();
    }

    public void DeActiveMoneyFlow()
    {
        isMoneyFlowActive = false;
    }

    #endregion

    private void LoadTurretSaveData()
    {
        var saveManager = SaveManager.Instance;

        turretSaveData = saveManager.LoadTurretData(slotIndex);

        currentTurret = turrets[turretSaveData.cannonIndex];

        LoadTotalLevelCount();

        LoadTurret();

        LoadInvestedMoney();

        LoadLevelText();

        CheckMaxLevel();
    }

    private void LoadLevelText()
    {
        if (turretSaveData.cannonIndex != 0)
        {
            levelText.gameObject.SetActive(true);
        }

        SetLevelText();
    }

    public void Lock()
    {
        if (!LevelManager.Instance.currentLevel.TutorialLevel || LevelManager.Instance.currentStage != LevelManager.Instance.currentLevel.Stages[0])
        {
            GetComponent<BoxCollider>().enabled = false;
        }

        SwitchRendererTo(false);

        priceText.enabled = false;

        lockArea.gameObject.SetActive(true);

        currentTurret = turrets[turretSaveData.cannonIndex];

        currentTurret.gameObject.SetActive(false);
    }

    public void Unlock()
    {
        if (!LevelManager.Instance.currentLevel.TutorialLevel || LevelManager.Instance.currentStage != LevelManager.Instance.currentLevel.Stages[0])
        {
            GetComponent<BoxCollider>().enabled = true;
        }

        SwitchRendererTo(true);

        priceText.enabled = true;

        lockArea.gameObject.SetActive(false);

        LoadTurretSaveData();
    }



    private void LoadInvestedMoney()
    {
        investedMoney = turretSaveData.investedMoney;

        SetPriceText();
    }

    private void LoadTurret()
    {
        currentTurret = turrets[turretSaveData.cannonIndex];

        currentTurret.SetLevel(turretSaveData.levelIndex);

        currentTurret.gameObject.SetActive(true);

        if (totalLevelCount == 0)
        {
            currentTurret.TurretDisable();
        }
    }

    public void UpdateTurret()
    {
        turretSaveData.cannonIndex++;

        currentTurret.gameObject.SetActive(false);

        currentTurret = turrets[turretSaveData.cannonIndex];

        currentTurret.gameObject.SetActive(true);

        SetPriceText();

        SetLevelText();

        ShowSmokeEffect();

        SetActiveLevelText();

        if (turretSaveData.cannonIndex == 1)
            currentTurret.TurretActivate();
    }

    private void ShowSmokeEffect()
    {
        EffectManager.Instance.ShowEffect(transform.position, PoolTag.PoffSmoke);
    }

    private bool CanMoneyInvest()
    {
        if ((lastMoneyInvestTime + moneyInvestRate) < Time.time)
            return true;
        return false;
    }

    private bool CanLevelUp()
    {
        if (investedMoney >= currentTurret.GetPrice())
            return true;
        return false;
    }

    private void LevelUp()
    {
        investedMoney = 0;

        int levelIndex = currentTurret.TurretLevelUp(this);

        UpdateLevelIndex(levelIndex);

        totalLevelCount++;
        SetLevelText();
        UpdateTotalLevelCount();

        ShowSmokeEffect();

        CheckMaxLevel();

        imageUpgrader.Refill();
        SetPriceText();
        DeActiveMoneyFlow();

        EffectManager.Instance.ShowLevelUpEffect(transform.position + Vector3.up * yPos + Vector3.right * xPos, currentTurret.turretData.LevelUpText, 2f);
    }

    private void CalculateMoneyInvestRate()
    {
        neededMoney = currentTurret.GetPrice() - investedMoney;

        moneyInvestRate = moneyInvestDuration / neededMoney;

        moneyInvestRate = Mathf.Clamp(moneyInvestRate, 0f, .033f);


    }

    private int CalculateLosePrice()
    {
        if (PlayerController.Instance.GetMoney() > 10)
        {
            return 1 + Mathf.RoundToInt(neededMoney / 100f);
        }

        return 1;
    }

    private void SetPriceText()
    {
        var money = currentTurret.GetPrice() - investedMoney;
        priceText.SetText(PriceConverter.Convert(money));

        UpdateInvestedMoney();
    }

    private void UpdateInvestedMoney()
    {
        turretSaveData.investedMoney = investedMoney;
    }

    private void UpdateLevelIndex(int index)
    {
        turretSaveData.levelIndex = index;
    }

    private void UpdateTotalLevelCount()
    {
        turretSaveData.totalLevelCount = totalLevelCount;
    }

    private void SetLevelText()
    {
        levelText.SetText(totalLevelCount + "");
    }

    private void SetActiveLevelText()
    {
        levelText.gameObject.SetActive(true);
    }

    private void CheckMaxLevel()
    {
        var lastTurret = turrets[turrets.Count - 1];

        if (currentTurret == lastTurret)
        {
            if (currentTurret.LevelIndex == currentTurret.turretDatas.Count - 1)
            {
                CloseGunSlot();

                imageUpgrader.canUpgrade = false;
            }
        }
    }

    private void LoadTotalLevelCount()
    {
        totalLevelCount = turretSaveData.totalLevelCount;
    }

    private void CloseGunSlot()
    {
        priceText.gameObject.SetActive(false);

        isClosed = true;
    }

    public void OnStageStart()
    {
        // LoadTurretSaveData();
    }

    public void OnStageEnd()
    {
        if (isMoneyFlowActive)
        {
            isMoneyFlowActive = false;
        }

        var saveManager = SaveManager.Instance;

        saveManager.SaveTurretData(slotIndex, turretSaveData);
    }

    public void OnGameFail()
    {
        var saveManager = SaveManager.Instance;

        saveManager.SaveTurretData(slotIndex, turretSaveData);
    }
}
