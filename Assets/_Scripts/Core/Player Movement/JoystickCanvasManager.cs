using UnityEngine;

public class JoystickCanvasManager : SingletonMonoBehaviour<JoystickCanvasManager>
{
    [Header("DEPENDENCIES")]
    public Joystick joystick;

    protected override void Awake()
    {
        base.Awake();
    }
}
