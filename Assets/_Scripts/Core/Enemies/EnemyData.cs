using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Enemy Data", fileName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] float health;
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] float scale;

    public float Health { get => health; }
    public float Damage { get => damage; }
    public float Speed { get => speed; }
    public float Scale { get => scale; }
}
