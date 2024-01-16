using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PriceGainer : MonoBehaviour
{
    [Header("Animation Colors")]
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;
    [Header("Animation Values")]
    [SerializeField] float yPosition;
    [SerializeField] float initYPosition;
    [SerializeField] float animationDuration;
    [Header("Dependies")]
    [SerializeField] TextMeshProUGUI text;

    private Tween positionTween;
    private Tween colorTween;

    private void OnEnable()
    {
        text.color = startColor;

        transform.position += Vector3.up * initYPosition;

        GainAnimation();
    }

    private void OnDisable()
    {
        colorTween.Kill();
        positionTween.Kill();

        transform.localScale = Vector3.one;
    }

    private void GainAnimation()
    {
        positionTween = transform.DOMoveY(yPosition, animationDuration).Play();

        colorTween = text.DOColor(endColor, animationDuration).SetEase(Ease.InQuad).Play();
    }

    public void SetCount(float count)
    {
        text.SetText("$" + count.ToString("0.0"));
    }
    
    public void SetCount(int count)
    {
        text.SetText("$" + count.ToString());
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
}
