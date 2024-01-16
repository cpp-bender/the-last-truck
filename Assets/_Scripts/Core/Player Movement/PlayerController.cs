using UnityEngine;
using DG.Tweening;

public class PlayerController : SingletonMonoBehaviour<PlayerController>, IStageController, IGameFail, IGameController
{
    [Header("COMPONENTS"), Space(5f)]
    public Animator animator;
    public Rigidbody body;

    [Header("DEPENDENCIES"), Space(5f)]
    public Joystick joystick;
    public PlayerMovementData playerMovementData;

    [Header("DEBUG")]
    public PlayerSaveData playerSaveData;
    public bool canMove = true;

    private SaveManager saveManager;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        saveManager = SaveManager.Instance;

        SwitchAnimation("Idle");

        LoadMoney();
    }

    private void FixedUpdate()
    {
        OnMove();
    }

    public void OnMove()
    {
        if (!canMove)
        {
            return;
        }

        var horizontal = joystick.Horizontal;
        var vertical = joystick.Vertical;

        if (horizontal == 0f && vertical == 0f)
        {
            body.velocity = Vector3.zero;
        }

        Vector3 dir = Vector3.forward * vertical + Vector3.right * horizontal;
        body.AddForce(dir * playerMovementData.MoveSpeed * Time.fixedDeltaTime, playerMovementData.ForceMode);

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), playerMovementData.RotateSpeed * Time.deltaTime);
        }
    }

    public void SwitchAnimation(string name)
    {
        animator.SetTrigger(name);
    }

    private void LoadMoney()
    {
        if (LevelManager.Instance.currentLevel.TutorialLevel && LevelManager.Instance.currentLevel.Stages[0] == LevelManager.Instance.currentStage)
        {
            saveManager.SavePlayerData(new PlayerSaveData(95));
        }

        playerSaveData = saveManager.LoadPlayerData();

        MoneyUI.Instance.SetMoneyText("$" + Mathf.FloorToInt(playerSaveData.money).ToString());
    }

    public void GainMoney(float price)
    {
        playerSaveData.money += price;

        MoneyUI.Instance.SetMoneyText("$" + Mathf.FloorToInt(playerSaveData.money).ToString());
    }

    public int LoseMoney(int price)
    {
        if ((playerSaveData.money - price) >= 0)
        {
            playerSaveData.money -= price;

            if (!LevelManager.Instance.currentLevel.TutorialLevel || LevelManager.Instance.currentLevel.Stages[0] != LevelManager.Instance.currentStage)
            {
                saveManager.SavePlayerData(playerSaveData);
            }

            MoneyUI.Instance.SetMoneyText("$" + Mathf.FloorToInt(playerSaveData.money).ToString());

            return price;
        }

        return 0;
    }

    public int GetMoney()
    {
        return Mathf.FloorToInt(playerSaveData.money);
    }

    public void OnStageStart()
    {
        joystick.ResetHandle();

        joystick.ResetInput();

        joystick.SwitchObjectTo(true);

        canMove = true;

        SwitchAnimation("Idle");

        LoadMoney();
    }

    public void OnStageEnd()
    {
        DOMoveToCenter();

        saveManager.SavePlayerData(playerSaveData);
    }

    public void DOMoveToCenter()
    {
        Sequence stageSeq = DOTween.Sequence();

        Vector3 truckCenter = new Vector3(0f, 1.8f, 0f);

        var rotation = (Quaternion.LookRotation(truckCenter - transform.position).normalized).eulerAngles;

        float dist = (truckCenter - transform.position).magnitude;

        Tween rotateTween = transform.DORotate(rotation, .2f);

        Tween moveTween = transform.DOMove(truckCenter, .5f * dist);

        stageSeq.Append(rotateTween).Join(moveTween);

        stageSeq
            .OnStart(delegate
            {
                canMove = false;

                joystick.SwitchObjectTo(false);

                SwitchAnimation("Run");
            })
            .OnComplete(delegate
            {
                SwitchAnimation("Idle");
            })
            .Play();
    }

    public Tween MoveCenterRoutine()
    {
        Sequence stageSeq = DOTween.Sequence();

        Vector3 truckCenter = new Vector3(0f, 1.8f, 0f);

        var rotation = (Quaternion.LookRotation(truckCenter - transform.position).normalized).eulerAngles;

        float dist = (truckCenter - transform.position).magnitude;

        Tween rotateTween = transform.DORotate(rotation, .2f);

        Tween moveTween = transform.DOMove(truckCenter, .5f * dist);

        stageSeq.Append(rotateTween).Join(moveTween);

        stageSeq
            .OnStart(delegate
            {
                canMove = false;

                SwitchAnimation("Run");
            })
            .OnComplete(delegate
            {
                if (!Input.GetMouseButton(0))
                {
                    SwitchAnimation("Idle");
                }

                canMove = true;
            });

        return stageSeq;
    }

    public void OnGameFail()
    {
        canMove = false;

        saveManager.SavePlayerData(playerSaveData);
    }

    public void OnLevelStart()
    {

    }

    public void OnLevelEnd()
    {
        TransformMoneyToGem();
    }

    private void TransformMoneyToGem()
    {
        GameManager.instance.SetGem(Mathf.RoundToInt(playerSaveData.money));
    }
}
