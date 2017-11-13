using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAnimation : MonoBehaviour {
    Animator soldierAnimator;
    UnitConfig unitConfig;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;
    public UnitConfig target;
    public Transform projectileStartPos;
    MapConfig mapConfig;

    AudioSource audioSource;


    void Start () {
        soldierAnimator = GetComponent<Animator>();
        unitConfig = GetComponentInParent<UnitConfig>();
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = unitConfig.unitWeapon.weaponSoundShoot;
    }
	
	void Update () {
        if (!unitConfig.isMoving)
        {
            soldierAnimator.SetInteger("state", 0);
        }
        if (unitConfig.isMoving)
        {
            soldierAnimator.SetInteger("state", 1);
        }
        if (unitConfig.isSprinting)
        {
            soldierAnimator.SetInteger("state", 2);
        }
        if (unitConfig.isShooting)
        {
            soldierAnimator.SetInteger("state", 3);

            transform.parent.LookAt(TurnSystem.selectedTarget.transform.position);
            Vector3 eulerAngles = transform.parent.rotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;

            // Set the altered rotation back
            transform.parent.rotation = Quaternion.Euler(eulerAngles);
        }
        if(soldierAnimator.GetInteger("state") > 0 && soldierAnimator.GetInteger("state") != 3) // HACK: What!?
        {
            direction = transform.root.position - lastPosition;
            lastPosition = transform.root.position;
            lookRotation = Quaternion.LookRotation((direction == Vector3.zero) ? Vector3.forward : direction);
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, lookRotation, Time.deltaTime * 10);
        }
        else if(soldierAnimator.GetInteger("state") != 3)
        {
            Quaternion a = transform.parent.rotation;
            Quaternion b = Quaternion.LookRotation((direction == Vector3.zero) ? Vector3.forward : direction);
            transform.parent.rotation = Quaternion.Lerp(a, b, Time.deltaTime * 10);
        }
    }
    public void ShootProjectile()
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
        audioSource.Play();


    }
    public void ProjectileHit()
    {
        //HACK: dealing damage should not be in animation
        TurnSystem.selectedTarget.health.TakeDamage(CalculationManager.damage, unitConfig.unitWeapon);
    }
    public void End()
    {
        mapConfig.turnSystem.KeyboardSelect(true, mapConfig.turnSystem.playerUnits,TurnSystem.selectedUnit);
        unitConfig.isShooting = false;
        unitConfig.actionPoints.SubtractAllActions();
    }
}
