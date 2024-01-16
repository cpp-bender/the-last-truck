using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var saveManager = (SaveManager)target;

        if (GUILayout.Button("Reset Truck Data"))
        {
            saveManager.ResetTruckDataOnGUI();

            Debug.Log("Truck data is reset");
        }

        if (GUILayout.Button("Reset Turret Data"))
        {
            saveManager.ResetTurretData();

            Debug.Log("Turret data is reset");
        }

        if (GUILayout.Button("Reset StageUI Data"))
        {
            saveManager.ResetStageUIData();

            Debug.Log("StageUI data is reset");
        }

        if (GUILayout.Button("Reset Money"))
        {
            saveManager.ResetPlayerData();

            Debug.Log("Money is reset");
        }
        
        if (GUILayout.Button("Reset Level Data"))
        {
            saveManager.ResetLevelData();

            Debug.Log("Level data is reset");
        }
        
        if (GUILayout.Button("Reset All"))
        {
            saveManager.ResetTruckDataOnGUI();
            saveManager.ResetTurretData();
            saveManager.ResetStageUIData();
            saveManager.ResetPlayerData();
            saveManager.ResetLevelData();
            PlayerPrefs.DeleteAll();

            Debug.Log("All is reset");
        }
    }
}
