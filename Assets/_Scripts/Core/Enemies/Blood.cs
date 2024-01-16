using System;
using DG.Tweening;
using UnityEngine;

public class Blood : MonoBehaviour
{
    private Material material;
    public Color bloodColor;

    private void OnEnable()
    {
        material = GetComponent<MeshRenderer>().material;
        
        if (PlatformController.Instance == null)
            return;
        
        transform.SetParent(PlatformController.Instance.transform);
    }

    private void OnDisable()
    {
        
    }

    public void ChangeColor()
    {
        material.color = bloodColor;

        TransparentColor();
    }

    public void SetColor(Color color)
    {
        bloodColor = color;
    }
    
    public void TransparentColor()
    {
        Color tempColor = bloodColor;
        tempColor.a = 0f;
        material.DOColor(tempColor, 1f).SetDelay(1f).SetEase(Ease.InQuad).Play();
    }
}
