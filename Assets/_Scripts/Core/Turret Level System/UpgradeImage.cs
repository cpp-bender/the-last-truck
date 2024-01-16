using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UpgradeImage : MonoBehaviour
{
    [Header("DEPENDENCIES"), Space(5f)]
    [SerializeField] GunSlot gunSlotParent;
    [SerializeField] UpgradeImageSettings imageSettings;

    [Header("DEBUG")]
    public bool canUpgrade;

    private Image upgradeImage;
    private Tween fillTween;
    private Tween unFillTween;

    private void Awake()
    {
        upgradeImage = GetComponent<Image>();
        canUpgrade = true;
    }

    public void Fill()
    {
        if (unFillTween != null)
        {
            unFillTween.Kill(false);
        }

        fillTween = DOTween.To(() => upgradeImage.fillAmount, x => upgradeImage.fillAmount = x, 1f, imageSettings.FillDuration);
        fillTween
            .SetAutoKill(true)
            .OnComplete(OnFillComplete)
            .Play();
    }

    public void Refill()
    {
        upgradeImage.fillAmount = 0f;

        Fill();
    }

    public void UnFill()
    {
        if (fillTween != null)
        {
            fillTween.Kill(false);
        }

        unFillTween = DOTween.To(() => upgradeImage.fillAmount, x => upgradeImage.fillAmount = x, 0f, imageSettings.UnFillDuration);
        unFillTween
            .SetAutoKill(true)
            .OnStart(OnUnFillComplete)
            .Play();
    }

    public void SwitchObjectTo(bool turnOn)
    {
        gameObject.SetActive(turnOn);
    }

    private void OnFillComplete()
    {
        gunSlotParent.ActiveMoneyFlow();
    }

    private void OnUnFillComplete()
    {
        gunSlotParent.DeActiveMoneyFlow();
    }
}
