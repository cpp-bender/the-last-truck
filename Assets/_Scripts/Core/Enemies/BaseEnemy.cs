using UnityEngine;

public abstract class BaseEnemy : KillableObject
{
    public EnemyData enemyData;

    protected TruckController truck;

    virtual protected void OnEnable()
    {
        truck = TruckController.Instance;
        currentHealth = MaxHealth();
        transform.localScale = enemyData.Scale * Vector3.one;
    }

    virtual protected float MaxHealth()
    {
        return enemyData.Health;
    }

    abstract public void Attack();

    abstract public void GiveDamage();
}
