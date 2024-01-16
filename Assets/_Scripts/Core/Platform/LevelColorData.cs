using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Level Color Data", fileName = "Level Color Data")]
public class LevelColorData : ScriptableObject
{
    [SerializeField] Material[] mainColors;
    [SerializeField] Material[] nearColors;
    [SerializeField] Material[] sideColors;
    [SerializeField] Material[] zombieColors;

    public Material[] MainColors { get => mainColors; }
    public Material[] NearColors { get => nearColors; }
    public Material[] SideColors { get => sideColors; }
    public Material[] ZombieColors { get => zombieColors; }
}
