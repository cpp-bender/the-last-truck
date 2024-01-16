using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Prefab Data", fileName = "Prefab Data")]
public class PrefabData : ScriptableObject
{
    [SerializeField] GameObject canon;
    [SerializeField] GameObject missile;
    [SerializeField] GameObject zombie;

    public GameObject Canon { get => canon; }
    public GameObject Missile { get => missile; }
    public GameObject Zombie { get => zombie; }
}
