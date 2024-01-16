using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[DefaultExecutionOrder(-200)]
public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    
    public TMP_FontAsset font;
    public string gameName;
    public bool gameHasGems;

    public Color mainColor;
    public Color gemColor;

    public GameObject[] levelCounts;
    public GameObject[] gemCountInLevelTexts;
    public GameObject[] gemCountTotalTexts;
    public GameObject[] gemIcons;
    public GameObject gameNameParentGameObject;
    public GameObject tutorialHand;
    public GameObject tutorialText;
    public GameObject[] panels;
    public GameObject restartButton;
    public GameObject successText;
    public GameObject failText;
    public GameObject nextButtonText;
    public GameObject multiplierText;
    public GameObject tutorialPanel;
    public GameObject joystick;
    public GameObject moneyCounter;


    public Color outline0;

    private bool isLevelStartedForUI;
    private bool isLevelCompletedForUI;
    private bool isLevelFailedForUI;
    private bool isRestartButtonPressed;
    private bool isNextLevelButtonPressed;

    private bool letCountGems;
    private float timerCountGems;
    private float timerForCreatingGems;
    private float speedCountGems;
    private int startGemCountInLevel;
    private int endGemCountInLevel;
    private int startGemCountTotal;
    private int endGemCountTotal;

    void Awake()
    {
        CreateInstance();
    }
    
    void Start()
    {
        isLevelStartedForUI = false;
        isNextLevelButtonPressed = false;
        isRestartButtonPressed = false;
        
        PrepareUI();
    }
    
    void Update()
    {
        LevelStartForUI();

        CountingGems();
    }

    public void SetSuccessText(string text)
    {
        successText.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    public void LevelStartForUI()
    {
        if (Input.GetMouseButtonDown(0) || TruckDeadState.Instance.IsTruckDead())
        {
            if (!isLevelStartedForUI)
            {
                if (isLevelStartedForUI)
                {
                    return;
                }

                gameNameParentGameObject.SetActive(false);
                tutorialHand.SetActive(false);
                tutorialText.SetActive(false);
                if (gameHasGems)
                {
                    gemIcons[0].SetActive(true);
                }
                restartButton.SetActive(true);

                GameManager.instance.isLevelStarted = true;
                GameManager.instance.isLevelCompleted = false;
                GameManager.instance.isLevelFailed = false;
                isLevelStartedForUI = true;
                
                gemIcons[0].SetActive(false);


                if (!LevelManager.Instance.currentLevel.TutorialLevel || LevelManager.Instance.currentStage != LevelManager.Instance.currentLevel.Stages[0])
                {
                    StartLevel();
                }
            }
        }
    }

    public void StartLevel()
    {
        var gameControllers = HyperUtilities.FindInterfacesOfType<IGameController>();

        foreach (var gameController in gameControllers)
        {
            gameController.OnLevelStart();
        }
    }

    public void LevelCompleteForUI(float delay)
    {
        if (!isLevelCompletedForUI)
        {
            StartCoroutine(WaitForLevelComplete(delay));

            isLevelCompletedForUI = true;
        }
    }

    #region LEGACY CODE

    private void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void PrepareUI()
    {
        for (int i = 0; i < levelCounts.Length; i++)
        {
            levelCounts[i].GetComponent<TextMeshProUGUI>().font = font;
            levelCounts[i].GetComponent<TextMeshProUGUI>().color = mainColor;
        }
        
        levelCounts[0].GetComponent<TextMeshProUGUI>().text = (GameManager.instance.currentLevel - 1).ToString();
        levelCounts[levelCounts.Length - 1].GetComponent<TextMeshProUGUI>().text = GameManager.instance.currentLevel.ToString();

        for (int i = 0; i < gemCountInLevelTexts.Length; i++)
        {
            gemCountInLevelTexts[i].GetComponent<TextMeshProUGUI>().font = font;
            gemCountInLevelTexts[i].GetComponent<TextMeshProUGUI>().color = mainColor;
            gemCountInLevelTexts[i].GetComponent<TextMeshProUGUI>().text = GameManager.instance.gemCountInLevel.ToString();

            if (!gameHasGems)
            {
                gemCountInLevelTexts[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < gemCountTotalTexts.Length; i++)
        {
            gemCountTotalTexts[i].GetComponent<TextMeshProUGUI>().font = font;
            gemCountTotalTexts[i].GetComponent<TextMeshProUGUI>().color = mainColor;
            gemCountTotalTexts[i].GetComponent<TextMeshProUGUI>().text = GameManager.instance.gemCountTotal.ToString();

            if (!gameHasGems)
            {
                gemCountTotalTexts[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < gemIcons.Length - 1; i++)
        {
            gemIcons[i].GetComponent<Image>().color = gemColor;

            if (!gameHasGems)
            {
                gemIcons[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < gameNameParentGameObject.transform.childCount; i++)
        {
            gameNameParentGameObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = gameName;
            gameNameParentGameObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().font = font;
        }

        gameNameParentGameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().color = mainColor;

        tutorialText.GetComponent<TextMeshProUGUI>().color = mainColor;
        tutorialText.GetComponent<TextMeshProUGUI>().font = font;

        successText.GetComponent<TextMeshProUGUI>().font = font;
        failText.GetComponent<TextMeshProUGUI>().font = font;
        nextButtonText.GetComponent<TextMeshProUGUI>().font = font;

        restartButton.GetComponent<Image>().color = mainColor;

        multiplierText.GetComponent<TextMeshProUGUI>().font = font;
        Color transparentMainColor = mainColor;
        transparentMainColor.a = 0f;
        multiplierText.GetComponent<TextMeshProUGUI>().color = transparentMainColor;

        if (!gameHasGems)
        {
            gemIcons[0].SetActive(false);
        }

        restartButton.SetActive(false);

        PrepareGameName();
    }
    private void PrepareGameName()
    {
        gameNameParentGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().outlineColor = outline0;
        gameNameParentGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().outlineWidth = 1f;

        gameNameParentGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().outlineWidth = 0.001f;
        gameNameParentGameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().outlineWidth = 0.001f;
        gameNameParentGameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().outlineWidth = 0.001f;
    }


    IEnumerator WaitForLevelComplete(float delay)
    {
        yield return new WaitForSeconds(delay);

        panels[0].SetActive(false);
        panels[1].SetActive(true);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(WaitForMultiply());
    }
    public void LevelFailForUI(float delay)
    {
        if (!isLevelFailedForUI)
        {
            StartCoroutine(WaitForLevelFail(delay));

            isLevelFailedForUI = true;
        }
    }
    IEnumerator WaitForLevelFail(float delay)
    {
        yield return new WaitForSeconds(delay);

        panels[0].SetActive(false);
        panels[2].SetActive(true);

        yield return new WaitForSeconds(1.25f);

        GameManager.instance.RestartLevel();
    }
    public void RefreshGemCountInLevel()
    {
        for (int i = 0; i < gemCountInLevelTexts.Length; i++)
        {
            gemCountInLevelTexts[i].GetComponent<TextMeshProUGUI>().text = GameManager.instance.gemCountInLevel.ToString();

            if (gemCountInLevelTexts[i].activeInHierarchy)
            {
                gemCountInLevelTexts[i].GetComponent<Animator>().SetTrigger("PopUp");
            }
        }
    }
    private void RefreshGemCountTotal()
    {
        for (int i = 0; i < gemCountTotalTexts.Length; i++)
        {
            gemCountTotalTexts[i].GetComponent<TextMeshProUGUI>().text = GameManager.instance.gemCountTotalTemp.ToString();
            
            if (gemCountTotalTexts[i].activeSelf)
            {
                gemCountTotalTexts[i].GetComponent<Animator>().SetTrigger("PopUp");
            }
        }
    }
    private void CountGems()
    {
        speedCountGems = 2f;
        letCountGems = true;
        startGemCountInLevel = GameManager.instance.gemCountInLevel;
        endGemCountInLevel = 0;
        startGemCountTotal = GameManager.instance.gemCountTotalTemp;
        endGemCountTotal = GameManager.instance.gemCountTotal;
    }
    private void CountingGems()
    {
        if (letCountGems)
        {
            timerCountGems += Time.deltaTime * speedCountGems;
            timerForCreatingGems += Time.deltaTime * speedCountGems;

            GameManager.instance.gemCountInLevel = (int) Mathf.Lerp(startGemCountInLevel, endGemCountInLevel, timerCountGems);
            GameManager.instance.gemCountTotalTemp = (int) Mathf.Lerp(startGemCountTotal, endGemCountTotal, timerCountGems);
            
            RefreshGemCountInLevel();
            RefreshGemCountTotal();

            if (timerCountGems >= 1f)
            {
                timerCountGems = 0f;
                letCountGems = false;
            }

            if (timerForCreatingGems >= 0.2f)
            {
                CreateFlyingGems();
                timerForCreatingGems = 0f;
            }
        }
    }
    private void CreateFlyingGems()
    {
        GameObject flyingGem = Instantiate(gemIcons[1], gemIcons[2].transform.position, Quaternion.identity, transform);
        flyingGem.transform.GetChild(0).gameObject.SetActive(false);
        // flyingGem.transform.GetChild(1).gameObject.SetActive(false);
        flyingGem.AddComponent<GemIcon>().FlyGem(1, 4f, 0.5f);
    }
    
    IEnumerator WaitForMultiply()
    {
        if (GameManager.instance.isMultiplied)
        {
            multiplierText.GetComponent<Animator>().SetTrigger("Multiply");

            yield return new WaitForSeconds(0.1f);
        
            RefreshGemCountInLevel();
        
            yield return new WaitForSeconds(0.3f);
        }
        
        yield return new WaitForSeconds(0.4f);
        
        CountGems();
    }
    
    public void NextLevelButton()
    {
        if (isNextLevelButtonPressed)
        {
            return;
        }
        
        GameManager.instance.NextLevel();
        
        isNextLevelButtonPressed = true;
    }
    public void RestartButton()
    {
        if (isRestartButtonPressed)
        {
            return;
        }
        
        GameManager.instance.RestartLevel();
        
        isRestartButtonPressed = true;
    }
    #endregion
}