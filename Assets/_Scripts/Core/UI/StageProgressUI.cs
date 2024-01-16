using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StageProgressUI : MonoBehaviour, IStageController
{
    [Header("Progress")]
    [SerializeField] Image fillImage;
    [SerializeField] RectTransform backGroundRectTransform;

    [Header("Splitter Image")]
    [SerializeField] Sprite stageSplitter;
    [SerializeField] float width;
    [SerializeField] float height;

    private int totalStageCount => LevelManager.Instance.GetLevelStageCount();
    private int currentStageCount => LevelManager.Instance.GetCurrentStageCount();
    private Vector3 truckPos => TruckController.Instance.transform.position;
    private Vector3 levelEndPos => LevelManager.Instance.levelEndTransform.position;

    private float fullDistance;

    private float fillOffset;
    private bool canFill;

    private void Start()
    {
        // InitStageSplitters();

        fillOffset = 1 / (float)totalStageCount;
        UpdateProgressFill(fillOffset * currentStageCount);

        fullDistance = GetDistance();
        canFill = true;
    }

    private void Update()
    {
        if(!canFill)
            return;
        
        float newDistance = GetDistance();
        float progressValue = Mathf.InverseLerp(fullDistance, 0f, newDistance);

        float startFillRate = 1f - (totalStageCount - (currentStageCount - 1)) * fillOffset;

        UpdateProgressFill(progressValue * fillOffset + startFillRate);
    }

    private float GetDistance()
    {
        return Vector3.Distance(truckPos, levelEndPos);
    }

    private void UpdateProgressFill(float fillAmount)
    {
        fillImage.fillAmount = fillAmount;
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a; Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }

    #region CreateSplitters
    private void InitStageSplitters()
    {
        for (int i = 0; i < totalStageCount; i++)
        {
            CreateSplitterImage(i);
        }
    }

    private void CreateSplitterImage(int index)
    {
        var splitGO = new GameObject();

        splitGO.transform.parent = backGroundRectTransform;
        splitGO.AddComponent<Image>().sprite = stageSplitter;

        var rectTransform = splitGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, backGroundRectTransform.rect.height + height);

        rectTransform.position = CalculateImageSplitterXPosition(index);
        Debug.Log(CalculateImageSplitterXPosition(index));
    }

    private Vector3 CalculateImageSplitterXPosition(int index)
    {
        var xOffset = backGroundRectTransform.rect.width / totalStageCount;

        return Vector3.right * xOffset * index;
    }
    #endregion

    public void OnStageStart()
    {
        canFill = true;
    }

    public void OnStageEnd()
    {
        canFill = false;
    }
}
