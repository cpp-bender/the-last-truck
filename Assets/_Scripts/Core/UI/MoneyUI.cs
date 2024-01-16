using UnityEngine;
using TMPro;

public class MoneyUI : SingletonMonoBehaviour<MoneyUI>
{
    [SerializeField] TextMeshProUGUI moneyText;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetMoneyText(string str)
    {
        moneyText.SetText(str);
    }
}
