using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckDeadState : SingletonMonoBehaviour<TruckDeadState>
{
    private bool isTruckDead = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public bool IsTruckDead()
    {
        return isTruckDead;
    }

    public void TruckDead()
    {
        isTruckDead = true;
    }

    public void TruckBorn()
    {
        isTruckDead = false;
    }
}
