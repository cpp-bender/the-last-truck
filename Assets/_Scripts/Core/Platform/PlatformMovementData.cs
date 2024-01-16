using UnityEngine;

[CreateAssetMenu(menuName ="The Last Truck/Platform Movement Data",fileName ="Platform Movement Data")]
public class PlatformMovementData : ScriptableObject
{
    [SerializeField] Vector3 moveDir = Vector3.back;
    [SerializeField] float moveSpeed = 10f;

    public Vector3 MoveDir { get => moveDir; set => moveDir = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
}
