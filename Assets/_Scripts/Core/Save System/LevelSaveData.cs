using System;

[Serializable]
public class LevelSaveData 
{
    public float inflationRate;

    public LevelSaveData(float inflationRate)
    {
        this.inflationRate = inflationRate;
    }

    public LevelSaveData()
    {

    }
}
