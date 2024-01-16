using System;

[Serializable]
public class PlayerSaveData 
{
    public float money;

    public PlayerSaveData(int money)
    {
        this.money = money;
    }

    public PlayerSaveData()
    {

    }
}
