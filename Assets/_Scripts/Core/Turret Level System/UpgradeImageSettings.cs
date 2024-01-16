using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Updrage Image Settings", fileName = "Upgrade Image Settings")]
public class UpgradeImageSettings : ScriptableObject
{
    [SerializeField] float fillDuration;
    [SerializeField] float unFillDuration;

    public float FillDuration { get => fillDuration; set => fillDuration = value; }
    public float UnFillDuration { get => unFillDuration; set => unFillDuration = value; }
}
