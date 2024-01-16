using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Player Movement Settings", fileName = "Player Movement Settings")]
public class PlayerMovementData : ScriptableObject
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 5f;
    [SerializeField] ForceMode forceMode = ForceMode.VelocityChange;

    public float MoveSpeed { get => moveSpeed; }
    public float RotateSpeed { get => rotateSpeed; }
    public ForceMode ForceMode { get => forceMode; }
}
