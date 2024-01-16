using System;

[Serializable]
public class StageUISaveData
{
    public int durabilityIndex;
    public int weaponDeckIndex;
    public int incomeIndex;

    public StageUISaveData()
    {

    }

    public StageUISaveData(int durabilityIndex, int weaponDeckIndex, int incomeIndex)
    {
        this.durabilityIndex = durabilityIndex;
        this.weaponDeckIndex = weaponDeckIndex;
        this.incomeIndex = incomeIndex;
    }
}
