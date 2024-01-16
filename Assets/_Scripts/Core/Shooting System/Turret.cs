using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using System;

public class Turret : MonoBehaviour
{
    [Header("DEPENDENCIES"), Space(5f)]
    public List<Transform> firePoints;
    public Transform yTurnerPlatform;
    public Transform xTurnerPlatform;

    [Header("Un Lock Effect Pivot"), Space(5f)]
    [SerializeField] Transform unLockPivotTransform;

    [Space(5f)]
    public List<RotatinLimit> rotationLimits;

    [Header("TURRET SETTINGS"), Space(5f)]
    public List<TurretData> turretDatas;
    public TurretData turretData;

    public TurretState CurrentState { get => currentState; }
    private TurretState currentState = TurretState.SeekEnemy;

    public int LevelIndex { get => levelIndex; }
    private int levelIndex = 0;


    protected Bullet currentBullet;
    protected Bullet[] currentBullets = new Bullet[3];

    protected Vector3 firstSeekPos;

    private BaseEnemy currentEnemy;
    private static Collider[] overlapResults = new Collider[25];
    private const float rootationSpeed = 20f;
    private float timer = 0f;
    private float buffValue = 1f;
    private float realEulerYRot = 0f;

    private Color[][] colors;

    private void Start()
    {
        InitPool();
        StartCoroutine(RunStateMachine());
        timer = turretData.FireRate;
    }

    private void InitPool()
    {
        turretData.BulletData.Prefab.GetComponent<Bullet>().Tag = turretData.BulletData.Tag;
        ObjectPooler.Instance.CreatePool(turretData.BulletData.Prefab, turretData.BulletData.Tag, 2);
    }

    public int GetPrice()
    {
        return Mathf.RoundToInt(turretData.Price * GameManager.instance.levelSaveData.inflationRate);
    }

    public void SetLevel(int index)
    {
        levelIndex = index;
        turretData = turretDatas[index];
    }

    #region State Machine
    private IEnumerator GetStateRoutine()
    {
        IEnumerator result = null;
        switch (currentState)
        {
            case TurretState.SeekEnemy:
                result = SeekEnemy();
                break;
            case TurretState.AttackEnemy:
                result = AttackEnemy();
                break;
            case TurretState.Idle:
                break;
            default:
                break;
        }

        return result;
    }

    private IEnumerator RunStateMachine()
    {
        while (true)
        {
            var stateRoutine = GetStateRoutine();

            yield return stateRoutine;
        }
    }

    private IEnumerator SeekEnemy()
    {
        while (currentState == TurretState.SeekEnemy)
        {
            //State Loop
            var hitCount = Physics.OverlapSphereNonAlloc(transform.position, turretData.Radius * buffValue, overlapResults, LayerMask.GetMask("Enemy"));
            var results = overlapResults.Take(hitCount);
            var enemies = results
                .Where(e => e.CompareTag("Enemy"))
                .Select(e => e.GetComponent<BaseEnemy>())
                .OrderByDescending(e => (e.transform.position - transform.position).sqrMagnitude);

            foreach (var enemy in enemies.ToArray())
            {
                if (CanTurretLookAtEnemy(enemy) && enemy.gameObject.activeSelf)
                {
                    currentEnemy = enemy;
                    firstSeekPos = currentEnemy.transform.position;
                    currentState = TurretState.AttackEnemy;
                    continue;
                }
            }

            yield return null;
        }

        yield return null;
    }

    private IEnumerator AttackEnemy()
    {
        //State Enter

        while (currentState == TurretState.AttackEnemy)
        {
            // Check enemy is not dead yet. If is not seek enemy.
            if (!currentEnemy.gameObject.activeSelf)
            {
                currentState = TurretState.SeekEnemy;
                continue;
            }

            // Check enemy is in turret range. If is not seek enemy.
            var sqrDistanceToTarget = (currentEnemy.transform.position - transform.position).sqrMagnitude;
            if (sqrDistanceToTarget > turretData.Radius * buffValue * turretData.Radius * buffValue)
            {
                currentEnemy = null;
                currentState = TurretState.SeekEnemy;
                continue;
            }

            timer += Time.deltaTime;

            // LookAt Enemy

            LookAtEnemy(currentEnemy.transform.position + Vector3.up * 0.5f);

            if (timer >= turretData.FireRate / buffValue)
            {
                //Start Attack
                Fire(currentEnemy);

                timer -= turretData.FireRate / buffValue;
            }

            //State Loop
            yield return null;
        }
        //State Exit
        yield return null;
    }

    public virtual void Fire(BaseEnemy enemy)
    {
        for (int i = 0; i < turretData.BarrelCount; i++)
        {
            GameObject bullet = ObjectPooler.Instance.DequeueFromPool(turretData.BulletData.Tag);

            currentBullet = bullet.GetComponent<Bullet>();
            currentBullet.Damage = turretData.Damage * buffValue;
            currentBullet.transform.position = firePoints[i].position;
            currentBullet.transform.forward = firePoints[i].forward;
            currentBullet.gameObject.SetActive(true);

            currentBullets[i] = currentBullet;
        }
    }
    #endregion

    #region Look Enemy
    protected virtual void LookAtEnemy(Vector3 enemyPos)
    {
        var lookPos = enemyPos - yTurnerPlatform.position;
        var baseRot = Quaternion.LookRotation(lookPos);

        YTurnerPlatformTurn(baseRot);
        XTurnerPlatformTurn(baseRot);
    }

    protected void YTurnerPlatformTurn(Quaternion baseRot)
    {
        var yRot = new Quaternion(yTurnerPlatform.rotation.x, baseRot.y, yTurnerPlatform.rotation.z, baseRot.w);
        yTurnerPlatform.rotation = Quaternion.Lerp(yTurnerPlatform.rotation, yRot, Time.deltaTime * rootationSpeed);

        //Car Shake Fixed;
        var yLocalRotation = yTurnerPlatform.localRotation;
        yLocalRotation.x = 0;
        yLocalRotation.z = 0;
        yTurnerPlatform.localRotation = yLocalRotation;
    }

    protected void XTurnerPlatformTurn(Quaternion baseRot)
    {
        var xRot = new Quaternion(baseRot.x, 0f, 0f, baseRot.w);
        xTurnerPlatform.localRotation = Quaternion.Lerp(xTurnerPlatform.rotation, xRot, Time.deltaTime * rootationSpeed);
        xTurnerPlatform.localEulerAngles -= Vector3.up * xTurnerPlatform.localEulerAngles.y;
        //firePoint.localEulerAngles = new Vector3(xTurnerPlatform.localEulerAngles.x / 2f, 0f, 0f);
    }

    private bool CanTurretLookAtEnemy(BaseEnemy enemy)
    {
        var lookPos = enemy.transform.position - yTurnerPlatform.position;
        var lookEulerRot = Quaternion.LookRotation(lookPos).eulerAngles;

        realEulerYRot = lookEulerRot.y;

        foreach (var rotationLimit in rotationLimits)
        {
            if (realEulerYRot >= rotationLimit.min && realEulerYRot <= rotationLimit.max)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Buff
    public void Buff()
    {
        if (currentState == TurretState.Idle)
            return;

        buffValue = 1.5f;
    }

    public void DeBuff()
    {
        if (currentState == TurretState.Idle)
            return;

        buffValue = 1f;
    }
    #endregion

    public void TurretActivate(float duration = 0.5f)
    {
        TurretDisable();

        currentState = TurretState.Idle;
        unLockPivotTransform.DOLocalRotateQuaternion(Quaternion.identity, duration)
            .SetEase(Ease.Linear)
            .Play()
            .OnComplete(() => currentState = TurretState.SeekEnemy);
    }

    public void TurretDisable(float duration = 0f)
    {
        unLockPivotTransform.DOLocalRotate(new Vector3(120f, 0, 0), duration)
            .SetEase(Ease.Linear)
            .Play()
            .OnComplete(() => currentState = TurretState.Idle);
    }

    public int TurretLevelUp(GunSlot gunSlot)
    {
        if (turretData.LookingChange)
        {
            gunSlot.UpdateTurret();

            return 0;
        }

        if (levelIndex < turretDatas.Count - 1)
        {
            levelIndex++;

            turretData = turretDatas[levelIndex];

            return levelIndex;
        }

        return -1;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.color = Color.cyan;

        if (currentEnemy != null)
        {
            Handles.DrawLine(xTurnerPlatform.position, currentEnemy.transform.position, 3f);
        }
    }
#endif
}

[Serializable]
public class RotatinLimit
{
    public float min;
    public float max;
}