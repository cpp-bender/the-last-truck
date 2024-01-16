using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Stage UI Data", fileName = "Stage UI Data")]
public class StagePriceData : ScriptableObject
{
    [SerializeField] List<int> weaponDeckPrices;
    [SerializeField] List<int> durabilityPrices;
    [SerializeField] List<int> incomePrices;

    public List<int> WeaponDeckPrices { get => weaponDeckPrices; }
    public List<int> DurabilityPrices { get => durabilityPrices; }
    public List<int> IncomePrices { get => incomePrices; }
}
