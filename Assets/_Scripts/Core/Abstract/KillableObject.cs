using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KillableObject : MonoBehaviour
{
    protected float currentHealth;
    public bool isDead;

    virtual public void TakeDamage(float damage = 1)
    {
        currentHealth -= damage;
        ScaleDown();
        DamageEffect();

        if (currentHealth <= 0 && !isDead)
        {
            EarnMoney();
            Death();
        }
    }

    abstract protected void Suicide();
    
    abstract protected void DamageEffect();

    abstract protected void Death();
    
    abstract protected void EarnMoney();
    
    abstract protected void ScaleDown();

}
