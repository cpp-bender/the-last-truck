using UnityEngine.AI;
using UnityEngine;

public class Zombie : BaseEnemy
{
    [Header("COMPONENTS"), Space(5f)]
    [SerializeField] Animator animator;
    [SerializeField] public SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Blood blood;

    [Header("DEBUG"), Space(5f)]
    public ZombieState zombieState;

    private const float ATTACKTHRESHOLD = 50f;


    private void Start()
    {
        agent.speed = enemyData.Speed;

        agent.enabled = false;

        zombieState = ZombieState.Search;

        animator.SetTrigger("Idle");
    }

    private void Update()
    {
        if (TruckController.Instance.isDead && zombieState != ZombieState.Stop)
        {
            Stop();
            return;
        }

        if (zombieState == ZombieState.Search)
        {
            Search();
        }
        else if (zombieState == ZombieState.Attack)
        {
            Attack();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.TRUCK))
        {
            Suicide();
        }
    }

    private void Search()
    {
        float dist = (TruckController.Instance.transform.position - transform.position).sqrMagnitude;

        if (dist <= ATTACKTHRESHOLD * ATTACKTHRESHOLD)
        {
            OnMoveStart();
        }
    }

    private void OnMoveStart()
    {
        transform.SetParent(null);

        // transform.localScale = Vector3.one;

        agent.enabled = true;

        zombieState = ZombieState.Attack;

        animator.SetTrigger("Run");
    }

    public override void Attack()
    {
        agent.SetDestination(TruckController.Instance.transform.position);
    }

    public void Stop()
    {
        if (!agent.isActiveAndEnabled)
            return;

        agent.SetDestination(transform.position);
        animator.SetTrigger("Idle");
        zombieState = ZombieState.Stop;
    }

    public override void GiveDamage()
    {
        TruckController.Instance.TakeDamage((enemyData.Damage));
    }

    protected override void EarnMoney()
    {
        EffectManager.Instance.ShowGainEffect(transform.position,  TruckController.Instance.income * enemyData.Health, 2f);
        PlayerGainPrice();
    }

    protected override void ScaleDown()
    {
        float healthPercentage = currentHealth / MaxHealth();
        transform.localScale = Vector3.one + healthPercentage * Vector3.one * (enemyData.Scale - 1f);
    }

    protected override void DamageEffect()
    {
        var bloodSplat = EffectManager.Instance.ShowEffect(transform.position, PoolTag.BloodSplatWide);
        var main = bloodSplat.GetComponent<ParticleSystem>().main;
        main.startColor = skinnedMeshRenderer.material.color;
        var mainChild = bloodSplat.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        mainChild.startColor = skinnedMeshRenderer.material.color;
    }

    protected override void Death()
    {
        var bloodEffect = EffectManager.Instance.ShowEffect(transform.position, PoolTag.Blood, 5f);
        bloodEffect.GetComponent<Blood>().SetColor(skinnedMeshRenderer.material.color);
        bloodEffect.GetComponent<Blood>().ChangeColor();

        zombieState = ZombieState.Search;
        agent.enabled = false;
        ObjectPooler.Instance.EnqueueToPool(PoolTag.ZombieSmall, gameObject);
    }

    private void PlayerGainPrice()
    {
        PlayerController.Instance.GainMoney(TruckController.Instance.income * enemyData.Health);
    }

    protected override void Suicide()
    {
        GiveDamage();
        Death();
    }
}