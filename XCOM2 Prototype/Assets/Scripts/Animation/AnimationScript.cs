using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour {
    public Animator animator;
    UnitConfig unitConfig;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;
    public UnitConfig target;
    public Transform projectileStartPos;

    AudioSource audioSource;

	void Start () {
        target = TurnSystem.selectedTarget;
        animator = GetComponent<Animator>();
        unitConfig = GetComponentInParent<UnitConfig>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource)
        {
            audioSource.clip = unitConfig.unitWeapon.weaponSoundShoot;
        }
	}
	
	void Update () {
        if (animator != null)
        {
            if (unitConfig.CheckUnitState(UnitConfig.UnitState.Idle))
            {
                animator.SetInteger("state", 0);
            }
            if (unitConfig.CheckUnitState(UnitConfig.UnitState.Walking))
            {
                animator.SetInteger("state", 1);
            }
            if (unitConfig.CheckUnitState(UnitConfig.UnitState.Sprinting))
            {
                animator.SetInteger("state", 2);
            }
            if (unitConfig.CheckUnitState(UnitConfig.UnitState.Shooting))
            {
                animator.SetInteger("state", 3);
                if (target == null)
                {
                    if (TurnSystem.selectedUnit = TurnSystem.selectedTarget)
                    {
                        target = TurnSystem.selectedUnit;
                    }
                    else
                    {
                        target = TurnSystem.selectedTarget;
                    }
                }
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
            //Death animation
            if ((unitConfig.CheckUnitState(UnitConfig.UnitState.Dead)))
            {
                animator.SetInteger("state", 4);
            }
            //walking rotation
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
    }

    public void SetAnimationState(int animationState)
    {
        animator.SetInteger("state", animationState);
    }
    public void AttackStart()
    {
        //When the projectile is supposed to shoot
        if (unitConfig.unitWeapon.weaponProjectile != null)
        {
            ParticleSystem projectileSystem = Instantiate(unitConfig.unitWeapon.weaponProjectile, projectileStartPos.position, transform.parent.rotation).GetComponent<ParticleSystem>();
            ParticleSystem.MainModule settings = projectileSystem.main;
            //ParticleSystem.SubEmittersModule subEmitter = projectileSystem.subEmitters;

            //ParticleSystem hitEmitter = Instantiate(target.unitClassStats.hitParticleSystem, projectileSystem.transform).GetComponent<ParticleSystem>();
            //hitEmitter.transform.localRotation = transform.parent.rotation;


            //subEmitter.AddSubEmitter(hitEmitter, ParticleSystemSubEmitterType.Collision, ParticleSystemSubEmitterProperties.InheritRotation);

            if(unitConfig.unitWeapon.particleColor.Length > 0)
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
        target.health.TakeDamage(unitConfig.unitWeapon);
    }

    public void AttackEnd()
    {
        unitConfig.SetUnitState(UnitConfig.UnitState.Idle);
        unitConfig.mapConfig.turnSystem.KeyboardSelect(true, unitConfig.mapConfig.turnSystem.playerUnits, unitConfig);
    }

    public void Death()// DESTROYS THE UNIT, Called at death animation end
    {
        unitConfig.health.KillUnit();
    }
}
