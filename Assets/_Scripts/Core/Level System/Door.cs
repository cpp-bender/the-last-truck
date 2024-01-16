using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour, IStageController
{
    [Header("DEPENDENCIES")]
    public GameObject rightPivot;
    public GameObject leftPivot;

    public BaseTweenData rightPivotTweenData;
    public BaseTweenData leftPivotTweenData;

    public void OnStageEnd()
    {
        OpenDoor();
    }

    public void OnStageStart()
    {
        ResetDoor();
    }

    private void ResetDoor()
    {
        rightPivot.transform.rotation = Quaternion.identity;

        leftPivot.transform.rotation = Quaternion.identity;
    }

    private void OpenDoor()
    {
        Sequence doorSequence = DOTween.Sequence();

        Tween rightPivotTween = rightPivotTweenData.GetTween(rightPivot);

        Tween leftPivotTween = leftPivotTweenData.GetTween(leftPivot);

        doorSequence.Append(rightPivotTween).Join(leftPivotTween);

        doorSequence.Play();
    }
}
