using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Turret Data", fileName = "Turret Data")]
public class TurretData : ScriptableObject
{
    [SerializeField] BulletData bulletData;
    [SerializeField] int barrelCount = 1;
    [SerializeField] float damage = 5f;
    [SerializeField] float radius = 10f;
    [SerializeField] float areaOfEffect = 10f;
    [SerializeField] float fireRate = .5f;
    [SerializeField] int price = 40;
    [SerializeField] bool lookingChange = false;
    [SerializeField] string levelUpText = "";

    public BulletData BulletData { get => bulletData; set => bulletData = value; }
    public int BarrelCount { get => barrelCount; set => barrelCount = value; }
    public float Damage { get => damage; set => damage = value; }
    public float Radius { get => radius; set => radius = value; }
    public float AreaOfEffect { get => areaOfEffect; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public int Price { get => price; set => price = value; }
    public bool LookingChange { get => lookingChange; }
    public string LevelUpText { get => levelUpText; }
}
