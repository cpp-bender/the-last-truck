using UnityEngine;

public static class PoolUtility
{
    private readonly static string[] tags = new string[]
    {
        "CanonBullet",
        "MissileBullet",
        "ZombieSmall",
        "ExplosionFireBallBlue",
        "PoffSmoke",
        "BloodSplatWide",
        "Blood",
        "PriceGain",
        "TruckExplosion",
        "ZombieMedium",
        "ZombieLarge",
        "TurretLevelUp"
    };

    public static string TagToString(PoolTag poolTag)
    {
        return tags[(int)poolTag];
    }
}
