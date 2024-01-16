using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Level Data", fileName = "Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] List<StageData> stages;
    [SerializeField] PlatformData platformData;
    [SerializeField] bool tutorialLevel = false;

    public List<StageData> Stages { get => stages; }
    public PlatformData PlatformData { get => platformData; }
    public bool TutorialLevel { get => tutorialLevel; }
}
