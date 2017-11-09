using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimation : MonoBehaviour {
    Animator zombieAnimation;
    UnitConfig unitConfig;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;
    public UnitConfig target;

    void Start () {
        zombieAnimation = GetComponent<Animator>();
        unitConfig = GetComponentInParent<UnitConfig>();

	}
	
	void Update () {
        if (!unitConfig.isMoving)
        {
            zombieAnimation.SetInteger("state", 0);
        }
        if (unitConfig.isMoving)
        {
            zombieAnimation.SetInteger("state", 1);
        }
        if (unitConfig.isSprinting)
        {
            zombieAnimation.SetInteger("state", 2);
        }
        if (unitConfig.isShooting)
        {
            zombieAnimation.SetInteger("state", 3);
            if(target != null)
            {
                transform.parent.LookAt(target.transform.position);
                Vector3 eulerAngles = transform.parent.rotation.eulerAngles;
                eulerAngles.x = 0;
                eulerAngles.z = 0;
                // Set the altered rotation back
                transform.parent.rotation = Quaternion.Euler(eulerAngles);
                
            }

        }
        if (unitConfig.isDead)
        {
            zombieAnimation.SetInteger("state", 4);
        }

        if (zombieAnimation.GetInteger("state") > 0) // HACK: What!?
        {
            direction = transform.root.position - lastPosition;
            lastPosition = transform.root.position;
            lookRotation = Quaternion.LookRotation((direction == Vector3.zero) ? Vector3.forward : direction);
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, lookRotation, Time.deltaTime * 10);
        }
        else
        {
            Quaternion a = transform.parent.rotation;
            Quaternion b = Quaternion.LookRotation((direction == Vector3.zero) ? Vector3.forward : direction);
            transform.parent.rotation = Quaternion.Lerp(a, b, Time.deltaTime * 10);
        }
    }
    public void ZombiePunch()
    {
        target.health.TakeDamage(CalculationManager.damage, unitConfig.unitWeapon);
    }
    public void End()
    {
        unitConfig.isShooting = false;
    }
    public void Death()
    {
        unitConfig.health.KillUnit();
        unitConfig.mapConfig.tileMap.ChangeGridColor(TurnSystem.selectedUnit.movePoints, TurnSystem.selectedUnit.actionPoints.actions, TurnSystem.selectedUnit);
    }
}

