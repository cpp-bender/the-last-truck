using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : Turret
{
    private const float missileMoveStartTime = 0.6f;
    private Missile currentMissile;
    public override void Fire(BaseEnemy enemy)
    {
        base.Fire(enemy);

        for (int i = 0; i < turretData.BarrelCount; i++)
        {
            currentMissile = currentBullets[i].GetComponent<Missile>();
            currentMissile.DamageRadius = turretData.AreaOfEffect;

            var extraXPos = 2f * i;
            StartCoroutine(currentMissile.SetTarget(enemy.transform, missileMoveStartTime, firstSeekPos, extraXPos, transform.position));
        }
    }

    protected override void LookAtEnemy(Vector3 enemyPos)
    {
        //var lookPos = enemyPos - yTurnerPlatform.position;
        //var baseRot = Quaternion.LookRotation(lookPos);

        //YTurnerPlatformTurn(baseRot);
    }

}
