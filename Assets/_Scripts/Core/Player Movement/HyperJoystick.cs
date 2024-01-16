using UnityEngine.EventSystems;
using UnityEngine;

public class HyperJoystick : Joystick
{
    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
        player = FindObjectOfType<PlayerController>();

        if (player == null)
        {
            Debug.LogError("Can't find player controller");
        }
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        player.SwitchAnimation("Run");
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        player.SwitchAnimation("Idle");
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}
