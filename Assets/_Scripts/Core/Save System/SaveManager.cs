using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : SingletonMonoBehaviour<SaveManager>, IGameController
{
    private const string TRUCKDATAPATH = "/TruckSaveFile.json";
    private const string TURRETDATAPATH = "/TurretSaveFile.json";
    private const string STAGEUIDATAPATH = "/StageUISaveFile.json";
    private const string PLAYERDATAPATH = "/PlayerSaveFile.json";
    private const string LEVELDATAPATH = "/LevelSaveFile.json";

    protected override void Awake()
    {
        base.Awake();
    }

    public void SaveTruckData(int deckIndex, int durability, float income)
    {
        TruckSaveData truck = new TruckSaveData();

        truck.deckIndex = deckIndex;
        truck.durability = durability;
        truck.income = income;

        string json = JsonUtility.ToJson(truck, true);
        string path = Application.persistentDataPath + TRUCKDATAPATH;

        File.WriteAllText(path, json);
    }

    public TruckSaveData LoadTruckData()
    {
        string path = Application.persistentDataPath + TRUCKDATAPATH;

        if (!File.Exists(path))
        {
            ResetTruckData();
        }

        string json = File.ReadAllText(Application.persistentDataPath + TRUCKDATAPATH);

        return JsonUtility.FromJson<TruckSaveData>(json);
    }

    public void SaveTurretData(int index, TurretSaveData turretSaveData)
    {
        var turrets = LoadTurretData();

        turrets[index] = turretSaveData;

        string json = JsonHelper.ToJson(turrets, true);
        string path = Application.persistentDataPath + TURRETDATAPATH;

        File.WriteAllText(path, json);
    }

    public TurretSaveData LoadTurretData(int index)
    {
        string path = Application.persistentDataPath + TURRETDATAPATH;

        if (!File.Exists(path))
        {
            ResetTurretData();
        }

        string json = File.ReadAllText(path);

        var turretSaveData = JsonHelper.FromJson<TurretSaveData>(json);

        return turretSaveData[index];
    }

    public TurretSaveData[] LoadTurretData()
    {
        string path = Application.persistentDataPath + TURRETDATAPATH;

        if (!File.Exists(path))
        {
            ResetTurretData();
        }

        string json = File.ReadAllText(path);

        var turretSaveData = JsonHelper.FromJson<TurretSaveData>(json);

        return turretSaveData;
    }

    public void SaveStageUIData(StageUISaveData stageUISaveData)
    {
        string path = Application.persistentDataPath + STAGEUIDATAPATH;
        string json = JsonUtility.ToJson(stageUISaveData);

        File.WriteAllText(path, json);
    }

    public StageUISaveData LoadStageUIData()
    {
        string path = Application.persistentDataPath + STAGEUIDATAPATH;

        if (!File.Exists(path))
        {
            ResetStageUIData();
        }

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<StageUISaveData>(json);
    }

    public void SavePlayerData(PlayerSaveData playerSaveData)
    {
        string path = Application.persistentDataPath + PLAYERDATAPATH;

        string json = JsonUtility.ToJson(playerSaveData);

        File.WriteAllText(path, json);
    }

    public PlayerSaveData LoadPlayerData()
    {
        string path = Application.persistentDataPath + PLAYERDATAPATH;

        if (!File.Exists(path))
        {
            ResetPlayerData();
        }

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<PlayerSaveData>(json);
    }

    public void SaveLevelData(LevelSaveData levelSaveData)
    {
        string path = Application.persistentDataPath + LEVELDATAPATH;

        string json = JsonUtility.ToJson(levelSaveData);

        File.WriteAllText(path, json);
    }

    public LevelSaveData LoadLevelData()
    {
        string path = Application.persistentDataPath + LEVELDATAPATH;

        if (!File.Exists(path))
        {
            ResetLevelData();
        }

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<LevelSaveData>(json);
    }

    public void ResetLevelData()
    {
        SaveLevelData(new LevelSaveData(1f));
    }

    public void ResetPlayerData(int moneyValue = 95)
    {
        Debug.Log(moneyValue);
        SavePlayerData(new PlayerSaveData(moneyValue));
    }

    public void ResetTruckData()
    {
        SaveTruckData(0, 100, 0.5f * GameManager.instance.levelSaveData.inflationRate);
    }

    public void ResetTruckDataOnGUI()
    {
        SaveTruckData(0, 100, 0.5f * 1f);
    }

    public void ResetTurretData()
    {
        var turrets = new List<TurretSaveData>();

        var turretSaveData = new TurretSaveData();

        turrets.Add(new TurretSaveData(0, 0, 0, 0));

        for (int i = 0; i < 6; i++)
        {
            turrets.Add(new TurretSaveData(0, 0, 0, 0));
        }

        string json = JsonHelper.ToJson(turrets.ToArray(), true);
        string path = Application.persistentDataPath + TURRETDATAPATH;

        File.WriteAllText(path, json);
    }

    public void ResetStageUIData()
    {
        StageUISaveData stageUISaveData = new StageUISaveData(0, 0, 0);

        SaveStageUIData(stageUISaveData);
    }

    public void OnLevelStart()
    {

    }

    public void OnLevelEnd()
    {
        ResetTruckData();
        ResetTurretData();
        ResetStageUIData();
    }


}
