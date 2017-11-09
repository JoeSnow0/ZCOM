using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour {
    Animator animator;
    UnitConfig unitConfig;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;
    public UnitConfig target;
    public Transform projectileStartPos;

    AudioSource audioSource;

	void Start () {
        animator = GetComponent<Animator>();
        unitConfig = GetComponentInParent<UnitConfig>();
        audioSource = GetComponent<AudioSource>();
	}
	
	void Update () {
        if (!unitConfig.isMoving)
        {
            animator.SetInteger("state", 0);
        }
        if (unitConfig.isMoving)
        {
            animator.SetInteger("state", 1);
        }
        if (unitConfig.isSprinting)
        {
            animator.SetInteger("state", 2);
        }
        if (unitConfig.isShooting)
        {
            animator.SetInteger("state", 3);
            if (target != null)
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
            animator.SetInteger("state", 4);
        }

        if (animator.GetInteger("state") > 0 && animator.GetInteger("state") != 3) // HACK: What!?
        {
            direction = transform.root.position - lastPosition;
            lastPosition = transform.root.position;
            lookRotation = Quaternion.LookRotation((direction == Vector3.zero) ? Vector3.forward : direction);
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, lookRotation, Time.deltaTime * 10);
        }
        else if (animator.GetInteger("state") != 3)
        {
            Quaternion a = transform.parent.rotation;
            Quaternion b = Quaternion.LookRotation((direction == Vector3.zero) ? Vector3.forward : direction);
            transform.parent.rotation = Quaternion.Lerp(a, b, Time.deltaTime * 10);
        }
    }

    public void AttackStart()//When the projectile is supposed to shoot
    {
        if (unitConfig.unitWeapon.weaponProjectile != null)
        {
            ParticleSystem.MainModule settings = Instantiate(unitConfig.unitWeapon.weaponProjectile, projectileStartPos.position, transform.parent.rotation).GetComponent<ParticleSystem>().main;

            settings.startColor = unitConfig.unitWeapon.particleColor[Random.Range(0, unitConfig.unitWeapon.particleColor.Length - 1)];
        }
        else
        {
            Debug.LogError("Particle system needed, drag particle system to weapon scriptable object");
        }

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void AttackHit()
    {
        target.health.TakeDamage(CalculationManager.damage, unitConfig.unitWeapon);
    }

    public void AttackEnd()
    {
        unitConfig.isShooting = false;
    }

    public void Death()// DESTROYS THE UNIT, Called at death animation end
    {
        unitConfig.health.KillUnit();
    }
}
