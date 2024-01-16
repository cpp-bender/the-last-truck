using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.TRUCK))
        {
            LevelManager.Instance.MoveNextPhase();
            GameManager.instance.StageComplete();
        }
    }

    public void SwitchRendererTo(bool turnOn)
    {
        meshRenderer.enabled = turnOn;
    }
}
