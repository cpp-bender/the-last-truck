using System.Collections;
using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TutorialManager : SingletonMonoBehaviour<TutorialManager>
{
    [Header("DEPENDENCIES")]
    public LevelManager levelManager;
    public UIManager uIManager;
    public TruckController truck;
    public TutorialUI tutorialUI;
    public GameObject indicatorFirst;
    public GameObject indicatorSecond;
    public GameObject indicatorThird;

    private Tween highlightTween;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (levelManager.currentLevel.TutorialLevel && levelManager.currentStage == levelManager.currentLevel.Stages[0])
        {
            StartCoroutine(TutorialRoutine());
        }
    }

    private IEnumerator TutorialRoutine()
    {
        yield return new WaitUntil(() => PrepareToUnlock());

        yield return new WaitUntil(() => truck.gunSlots[0].firstCanonUnlocked);

        yield return PlayerController.Instance.MoveCenterRoutine().Play().WaitForCompletion();

        yield return new WaitUntil(() => tutorialUI.PrepareToUpgrade());

        yield return new WaitUntil(() => truck.gunSlots[0].firstCannonUpgraded);

        yield return PlayerController.Instance.MoveCenterRoutine().Play().WaitForCompletion();

        yield return new WaitUntil(() => PrepareToStartGame());

        uIManager.StartLevel();
    }

    private bool PrepareToUnlock()
    {
        AdjustCam();

        TurnOffGameMenu();

        tutorialUI.PrepareToUnlock();

        TurnOnIndicator();

        AdjustGunSlotColliders();

        AdjustPriceTexts();

        DoFadeFirstSlot();


        void TurnOffGameMenu()
        {
            var gameMenu = uIManager.panels[0];
            gameMenu.SetActive(false);
        }

        void TurnOnIndicator()
        {
            indicatorFirst.SetActive(true);
        }

        void AdjustGunSlotColliders()
        {
            foreach (GunSlot gunSlot in truck.gunSlots)
            {
                gunSlot.GetComponent<BoxCollider>().enabled = false;
            }

            truck.gunSlots[0].GetComponent<BoxCollider>().enabled = true;
        }

        void AdjustPriceTexts()
        {
            truck.gunSlots[1].priceText.gameObject.SetActive(false);

            truck.gunSlots[2].priceText.gameObject.SetActive(false);
        }

        void AdjustCam()
        {
            var cam = Camera.main;

            cam.transform.position = new Vector3(0f, 10f, -5f);

            cam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        }

        void DoFadeFirstSlot()
        {
            highlightTween = truck.gunSlots[0].GetComponent<MeshRenderer>().material.DOColor(Color.green, .25f).SetLoops(-1, LoopType.Yoyo).Play();
        }

        return true;
    }

    private bool PrepareToStartGame()
    {
        ResetFirstSlot();

        AdjustCam();

        tutorialUI.PrepareToStart();

        StartCoroutine(AdjustIndicators());

        TurnOnGameMenu();

        TurnOnGunSlotColliders();

        AdjustPriceTexts();

        void ResetFirstSlot()
        {
            highlightTween.Kill(true);

            truck.gunSlots[0].GetComponent<MeshRenderer>().material.DOColor(Color.white, .5f).Play();
        }

        void TurnOnGameMenu()
        {
            var gameMenu = uIManager.panels[0];

            gameMenu.SetActive(true);
        }

        IEnumerator AdjustIndicators()
        {
            indicatorFirst.SetActive(false);

            indicatorSecond.SetActive(true);

            indicatorThird.SetActive(true);

            yield return new WaitForSeconds(5f);

            indicatorSecond.SetActive(false);

            indicatorThird.SetActive(false);
        }

        void TurnOnGunSlotColliders()
        {
            truck.gunSlots[0].GetComponent<BoxCollider>().enabled = true;
            truck.gunSlots[1].GetComponent<BoxCollider>().enabled = true;
            truck.gunSlots[2].GetComponent<BoxCollider>().enabled = true;
        }

        void AdjustCam()
        {
            var cam = Camera.main;

            Sequence camSeq = DOTween.Sequence();

            Vector3 pos = new Vector3(0f, 14f, -9.5f);

            Quaternion rot = Quaternion.Euler(45f, 0f, 0f);

            Tween moveTween = cam.transform.DOMove(pos, .5f).SetRecyclable(true);

            Tween rotateTween = cam.transform.DORotate(rot.eulerAngles, .5f).SetRecyclable(true);

            camSeq.Append(moveTween).Join(rotateTween);

            camSeq.Play();
        }

        void AdjustPriceTexts()
        {
            truck.gunSlots[1].priceText.gameObject.SetActive(true);

            truck.gunSlots[2].priceText.gameObject.SetActive(true);
        }

        return true;
    }
}
