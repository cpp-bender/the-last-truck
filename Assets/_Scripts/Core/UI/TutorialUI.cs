using System.Collections;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public GameObject unlockText;
    public GameObject upgradeText;
    public GameObject unlockOthersText;

    public void Switch(GameObject go, bool turnOn)
    {
        go.gameObject.SetActive(turnOn);
    }

    public void PrepareToUnlock()
    {
        gameObject.SetActive(true);

        unlockText.SetActive(true);

        upgradeText.SetActive(false);

        unlockOthersText.SetActive(false);
    }

    public bool PrepareToUpgrade()
    {
        unlockText.gameObject.SetActive(false);

        upgradeText.gameObject.SetActive(true);

        unlockOthersText.gameObject.SetActive(false);

        return true;
    }

    public void PrepareToStart()
    {
        unlockText.SetActive(false);
        upgradeText.SetActive(false);

        StartCoroutine(UnlockOthersTextRoutine());
    }

    private IEnumerator UnlockOthersTextRoutine()
    {
        unlockOthersText.SetActive(true);

        yield return new WaitForSeconds(5f);

        unlockOthersText.SetActive(false);
    }
}
