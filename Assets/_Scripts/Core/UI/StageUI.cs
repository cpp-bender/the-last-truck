using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class StageUI : SingletonMonoBehaviour<StageUI>, IStageController, IGameController
{
    [Header("DEPENDENCIES"), Space(5F)]
    [SerializeField] StagePriceData stagePriceData;
    [SerializeField] GameObject stageUIGO;
    [SerializeField] GameObject handImage;


    [Header("BUTTONS"), Space(5f)]
    [SerializeField] Button nextStageButton;
    [SerializeField] Button startStageButton;
    [SerializeField] Button durabilityButton;
    [SerializeField] Button weaponDeckButton;
    [SerializeField] Button incomeButton;

    [Header("TEXTS"), Space(5f)]
    [SerializeField] TextMeshProUGUI durabilityPriceText;
    [SerializeField] TextMeshProUGUI weaponDeckPriceText;
    [SerializeField] TextMeshProUGUI incomePriceText;
    [SerializeField] TextMeshProUGUI durabilityLevelText;
    [SerializeField] TextMeshProUGUI weaponDeckLevelText;
    [SerializeField] TextMeshProUGUI incomeLevelText;

    [Header("DEBUG")]
    public StageUISaveData stageUIData;

    private SaveManager saveManager;
    private PlayerController player;
    private TruckController truck;
    private bool isIncomeClickedForTutorial;
    private bool isNextStageClickedForTutorial;
    private Sequence handSeq;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        stageUIGO.SetActive(false);

        player = PlayerController.Instance;

        truck = TruckController.Instance;

        saveManager = SaveManager.Instance;

        LoadStageUIData();
    }

    private void LoadStageUIData()
    {
        stageUIData = saveManager.LoadStageUIData();
    }

    public void OnNextButtonClick()
    {
        if ((LevelManager.Instance.currentLevel.TutorialLevel && LevelManager.Instance.currentLevel.Stages[0] == LevelManager.Instance.currentStage) && !isNextStageClickedForTutorial)
        {
            isNextStageClickedForTutorial = true;
        }
        else
        {
            LevelManager.Instance.InitNextPhase();
        }
    }

    public void OnStartStageButtonClick()
    {
        stageUIGO.SetActive(false);
        startStageButton.gameObject.SetActive(false);
        nextStageButton.gameObject.SetActive(true);

        TruckDeadState.Instance.TruckBorn();
        PlatformController.Instance.MoveThePlatform();

        var sideSpawnPoints = FindObjectsOfType<SideSpawnPoint>();

        foreach (var sideSpawnPoint in sideSpawnPoints)
        {
            StartCoroutine(sideSpawnPoint.ZombieSpawnRoutine());
        }
    }

    public void OnDurabilityButtonClick()
    {
        var durabilityPrice = Mathf.RoundToInt(stagePriceData.DurabilityPrices[stageUIData.durabilityIndex] * GameManager.instance.levelSaveData.inflationRate);

        if (player.LoseMoney(durabilityPrice) > 0)
        {
            UpdateDurabilityPriceText(1);

            UpdateDurabilityLevelText();

            truck.UpdateDurability();

            UpdateButtonsInteractability();

            saveManager.SaveStageUIData(stageUIData);
        }
    }

    public void OnWeaponDeckButtonClick()
    {
        var weaponDeckPrice = Mathf.RoundToInt(stagePriceData.WeaponDeckPrices[stageUIData.weaponDeckIndex] * GameManager.instance.levelSaveData.inflationRate);

        if (player.LoseMoney(weaponDeckPrice) > 0)
        {
            UpdateWeaponDeckPriceText(1);

            UpdateWeaponDeckLevelText();

            truck.UpdateWeaponDeck();

            UpdateButtonsInteractability();

            saveManager.SaveStageUIData(stageUIData);
        }
    }

    public void OnIncomeButtonClick()
    {
        var incomePrice = Mathf.RoundToInt(stagePriceData.IncomePrices[stageUIData.incomeIndex] * GameManager.instance.levelSaveData.inflationRate);

        if (player.LoseMoney(incomePrice) > 0)
        {
            UpdateIncomePriceText(1);

            UpdateIncomeLevelText();

            truck.UpdateIncome();

            UpdateButtonsInteractability();

            saveManager.SaveStageUIData(stageUIData);
        }

        if ((LevelManager.Instance.currentLevel.TutorialLevel && LevelManager.Instance.currentLevel.Stages[0] == LevelManager.Instance.currentStage) && !isIncomeClickedForTutorial)
        {
            isIncomeClickedForTutorial = true;
        }
    }

    public void OnStageEnd()
    {
        if (LevelManager.Instance.HasStage())
        {
            stageUIGO.SetActive(true);

            UpdateStageUI();

            StartCoroutine(WaitForStageTimeOut());
        }
        else
        {
            StartCoroutine(WaitForLevelEnd());
        }
    }

    public void OnStageStart()
    {
        stageUIGO.SetActive(false);
        StopAllCoroutines();
    }

    public void OnLevelStart()
    {
        if (TruckDeadState.Instance.IsTruckDead())
        {
            stageUIGO.SetActive(true);
            startStageButton.gameObject.SetActive(true);
            nextStageButton.gameObject.SetActive(false);

            UpdateStageUI();
        }
    }

    public void OnLevelEnd()
    {

    }

    private void UpdateStageUI()
    {
        UpdateDurabilityPriceText();
        UpdateDurabilityLevelText();
        UpdateDurabilityButton();

        UpdateWeaponDeckPriceText();
        UpdateWeaponDeckLevelText();
        UpdateWeaponDeckButton();

        UpdateIncomePriceText();
        UpdateIncomeLevelText();
        UpdateIncomeButton();
    }

    private void UpdateButtonsInteractability()
    {
        UpdateDurabilityButton();

        UpdateWeaponDeckButton();

        UpdateIncomeButton();
    }

    private void UpdateDurabilityButton()
    {
        var player = PlayerController.Instance;

        if (stageUIData.durabilityIndex == stagePriceData.DurabilityPrices.Count - 1)
        {
            durabilityButton.interactable = false;
            return;
        }

        if (player.playerSaveData.money >= Mathf.RoundToInt(stagePriceData.DurabilityPrices[stageUIData.durabilityIndex] * GameManager.instance.levelSaveData.inflationRate))
        {
            durabilityButton.interactable = true;
        }
        else
        {
            durabilityButton.interactable = false;
        }
    }

    private void UpdateWeaponDeckButton()
    {
        var player = PlayerController.Instance;

        if (stageUIData.weaponDeckIndex == stagePriceData.WeaponDeckPrices.Count - 1)
        {
            weaponDeckButton.interactable = false;
            return;
        }

        if (player.playerSaveData.money >= Mathf.RoundToInt(stagePriceData.WeaponDeckPrices[stageUIData.weaponDeckIndex] * GameManager.instance.levelSaveData.inflationRate))
        {
            weaponDeckButton.interactable = true;
        }
        else
        {
            weaponDeckButton.interactable = false;
        }
    }

    private void UpdateIncomeButton()
    {
        var player = PlayerController.Instance;

        if (stageUIData.incomeIndex == stagePriceData.IncomePrices.Count - 1)
        {
            incomeButton.interactable = false;
            return;
        }

        if (player.playerSaveData.money >= Mathf.RoundToInt(stagePriceData.IncomePrices[stageUIData.incomeIndex] * GameManager.instance.levelSaveData.inflationRate))
        {
            incomeButton.interactable = true;
        }
        else
        {
            incomeButton.interactable = false;
        }
    }

    private void UpdateDurabilityPriceText(int index = 0)
    {
        if (index + stageUIData.durabilityIndex < stagePriceData.DurabilityPrices.Count)
        {
            stageUIData.durabilityIndex = (index + stageUIData.durabilityIndex);
        }

        if (stageUIData.durabilityIndex == stagePriceData.DurabilityPrices.Count - 1)
        {
            durabilityPriceText.text = "Max";
        }
        else
        {
            durabilityPriceText.text = PriceConverter.Convert(stagePriceData.DurabilityPrices[stageUIData.durabilityIndex]
                * GameManager.instance.levelSaveData.inflationRate);
        }
    }

    private void UpdateWeaponDeckPriceText(int index = 0)
    {
        if (index + stageUIData.weaponDeckIndex < stagePriceData.WeaponDeckPrices.Count)
        {
            stageUIData.weaponDeckIndex = (index + stageUIData.weaponDeckIndex);
        }

        if (stageUIData.weaponDeckIndex == stagePriceData.WeaponDeckPrices.Count - 1)
        {
            weaponDeckPriceText.text = "Max";
        }
        else
        {
            weaponDeckPriceText.text = PriceConverter.Convert(stagePriceData.WeaponDeckPrices[stageUIData.weaponDeckIndex]
                * GameManager.instance.levelSaveData.inflationRate);
        }
    }

    private void UpdateIncomePriceText(int index = 0)
    {
        if (index + stageUIData.incomeIndex < stagePriceData.IncomePrices.Count)
        {
            stageUIData.incomeIndex = (index + stageUIData.incomeIndex);
        }

        if (stageUIData.incomeIndex == stagePriceData.IncomePrices.Count - 1)
        {
            incomePriceText.text = "Max";
        }
        else
        {
            incomePriceText.text = PriceConverter.Convert(stagePriceData.IncomePrices[stageUIData.incomeIndex]
                * GameManager.instance.levelSaveData.inflationRate);
        }
    }

    private void UpdateDurabilityLevelText()
    {
        durabilityLevelText.text = "Lv " + $"{stageUIData.durabilityIndex + 1}";
    }

    private void UpdateWeaponDeckLevelText()
    {
        weaponDeckLevelText.text = "Lv " + $"{stageUIData.weaponDeckIndex + 1}";
    }

    private void UpdateIncomeLevelText()
    {
        incomeLevelText.text = "Lv " + $"{stageUIData.incomeIndex + 1}";
    }

    IEnumerator WaitForStageTimeOut()
    {
        if (LevelManager.Instance.currentLevel.TutorialLevel && LevelManager.Instance.currentLevel.Stages[0] == LevelManager.Instance.currentStage)
        {
            StartCoroutine(PlatformController.Instance.CheckPlatformEdges());

            yield return new WaitUntil(() => PrepareIncomeTutorial());

            DoClickIncome();

            yield return new WaitUntil(() => isIncomeClickedForTutorial);

            saveManager.SavePlayerData(PlayerController.Instance.playerSaveData);

            handSeq.Kill(false);

            yield return new WaitUntil(() => PrepareNextStageTutorial());

            DoClickNextStage();

            yield return new WaitUntil(() => isNextStageClickedForTutorial);

            handSeq.Kill(false);

            StopCoroutine(PlatformController.Instance.CheckPlatformEdges());


            LevelManager.Instance.InitNextPhase();



            bool PrepareIncomeTutorial()
            {
                handImage.SetActive(true);

                handImage.transform.SetParent(incomeButton.transform);

                handImage.GetComponent<RectTransform>().localPosition = Vector3.zero + Vector3.down * 100f + Vector3.right * -200f;

                weaponDeckButton.interactable = false;
                durabilityButton.interactable = false;
                nextStageButton.interactable = false;
                incomeButton.interactable = true;

                return true;
            }


            bool PrepareNextStageTutorial()
            {
                handImage.SetActive(true);

                handImage.transform.SetParent(nextStageButton.transform);
                
                handImage.GetComponent<RectTransform>().localPosition = Vector3.zero + Vector3.down * 100f + Vector3.right * -250f;

                nextStageButton.interactable = true;

                return true;
            }


            void DoClickIncome()
            {
                handSeq = DOTween.Sequence();

                Tween rotateTween = handImage.transform.DORotate(new Vector3(-30f, 0f, 0f), .5f);

                Tween scaleTween = handImage.transform.DOScale(.8f, 1f);

                handSeq.Append(rotateTween).Join(null)
                    .OnKill(delegate
                    {
                        handImage.SetActive(false);
                    })
                    .SetLoops(-1, LoopType.Yoyo);

                handSeq.Play();
            }


            void DoClickNextStage()
            {
                handSeq = DOTween.Sequence();

                Tween rotateTween = handImage.transform.DORotate(new Vector3(-35f, 0f, 0f), .5f);

                Tween scaleTween = handImage.transform.DOScale(.8f, 1f);

                handSeq.Append(rotateTween).Join(null)
                    .OnKill(delegate
                    {
                        handImage.SetActive(false);
                    })
                    .SetLoops(-1, LoopType.Yoyo);

                handSeq.Play();
            }
        }

        else
        {
            yield return new WaitForSeconds(10f);

            LevelManager.Instance.InitNextPhase();
        }
    }

    IEnumerator WaitForLevelEnd()
    {
        yield return new WaitForSeconds(4f);

        GameManager.instance.LevelComplete();
    }
}
