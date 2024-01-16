using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurretLevelUpEffect : MonoBehaviour
{
    [Header("Animation Colors")]
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;
    [Header("Animation Values")]
    [SerializeField] float yPosition;
    [SerializeField] float animationDuration;
    [SerializeField] TextMeshProUGUI text;

    private Tween positionTween;
    private Tween colorTween;

    private void OnEnable()
    {
        KillOldTweens();

        text.color = startColor;

        GainAnimation();
    }

    private void OnDisable()
    {
        InitTransformSettings();
    }

    private void GainAnimation()
    {
        positionTween = transform.DOMove(transform.position + Vector3.up * yPosition, animationDuration).Play();

        colorTween = text.DOColor(endColor, animationDuration).SetEase(Ease.InQuad).Play();
    }

    private void KillOldTweens()
    {
        positionTween.Kill();
        colorTween.Kill();
    }

    private void InitTransformSettings()
    {
        transform.localScale = Vector3.one;

        transform.position = Vector3.zero;
    }

    public void SetText(string text)
    {
        this.text.SetText(text);
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
}
