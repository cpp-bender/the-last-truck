using System.Collections;
using System.Linq;
using UnityEngine;

public class Missile : Bullet
{
    public Transform target = null;
    public Vector3 missileTurretposition;

    private const float moveSpeed = 15f;
    private const float rootationSpeed = 10f;

    public float DamageRadius { get => damageRadius; set => damageRadius = value; }
    public float damageRadius;
    public float extraXPos;

    private Vector3 lastTargetPosition;
    private Vector3 firstSeekPos;

    private static Collider[] overlapResults = new Collider[10];

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        if (target.gameObject.activeSelf)
        {
            lastTargetPosition = target.position;
            lastTargetPosition.y = 0f;

            LookAtTarget(lastTargetPosition + Vector3.right * extraXPos);
        }
        else
        {
            LookAtTarget(firstSeekPos + Vector3.right * extraXPos);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        target = null;
        StopAllCoroutines();
    }

    public IEnumerator SetTarget(Transform target, float duration, Vector3 firstSeekPos, float extraXPos, Vector3 missileTurretposition)
    {
        yield return new WaitForSeconds(duration);

        rb.velocity -= rb.velocity;

        this.target = target;
        this.missileTurretposition = missileTurretposition;
        this.firstSeekPos = firstSeekPos;
        this.extraXPos = extraXPos;


        if (gameObject.activeSelf)
            StartCoroutine(MoveToTarget());
    }

    public IEnumerator MoveToTarget()
    {
        yield return new WaitForSeconds(0f);

        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        StartCoroutine(MoveToTarget());
    }

    public void LookAtTarget(Vector3 targetPos)
    {
        var lookPos = targetPos - transform.position;
        var baseRot = Quaternion.LookRotation(lookPos);

        transform.rotation = Quaternion.Lerp(transform.rotation, baseRot, Time.deltaTime * rootationSpeed);
    }

    protected override void GiveDamage(Collider other)
    {
        EffectManager.Instance.ShowEffect(transform.position, PoolTag.ExplosionFireBallBlue);

        var hitCount = Physics.OverlapSphereNonAlloc(transform.position, damageRadius, overlapResults, LayerMask.GetMask("Enemy"));
        var results = overlapResults.Take(hitCount);

        foreach (var enemy in results)
        {
            enemy.GetComponent<BaseEnemy>().TakeDamage(damage);
        }
    }

    protected override void ConnectWithGround(Collider other)
    {
        GiveDamage(other);
        base.ConnectWithGround(other);
    }
}
