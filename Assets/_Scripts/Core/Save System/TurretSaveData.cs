using System;

[Serializable]
public class TurretSaveData
{
    public int cannonIndex = 0;
    public int levelIndex = 0;
    public int investedMoney = 0;
    public int totalLevelCount = 0;

    public TurretSaveData(int cannonIndex, int levelIndex, int investedMoney, int totalLevelCount)
    {
        this.cannonIndex = cannonIndex;
        this.levelIndex = levelIndex;
        this.investedMoney = investedMoney;
        this.totalLevelCount = totalLevelCount;
    }

    public TurretSaveData()
    {

    }
}
