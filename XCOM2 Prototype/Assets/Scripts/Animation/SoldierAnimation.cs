using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAnimation : MonoBehaviour {
    Animator soldierAnimator;
    UnitConfig unit;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;
    public UnitConfig target;
    public Transform projectileStartPos;
    TurnSystem turnSystem;

    AudioSource audioSource;


    void Start () {
        soldierAnimator = GetComponent<Animator>();
        unit = GetComponentInParent<UnitConfig>();
        turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = unit.unitWeapon.weaponSoundShoot;
    }
	
	void Update () {
        if (!unit.isMoving)
        {
            soldierAnimator.SetInteger("state", 0);
        }
        if (unit.isMoving)
        {
            soldierAnimator.SetInteger("state", 1);
        }
        if (unit.isSprinting)
        {
            soldierAnimator.SetInteger("state", 2);
        }
        if (unit.isShooting)
        {
            soldierAnimator.SetInteger("state", 3);

            transform.parent.LookAt(target.transform.position);
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
        if (unit.unitWeapon.weaponProjectile != null)
        {
            ParticleSystem.MainModule settings = Instantiate(unit.unitWeapon.weaponProjectile, projectileStartPos.position, transform.parent.rotation).GetComponent<ParticleSystem>().main;

            settings.startColor = unit.unitWeapon.particleColor[Random.Range(0, unit.unitWeapon.particleColor.Length - 1)];
        }
        else
        {
            Debug.LogError("Particle system needed, drag particle system to weapon scriptable object");
        }
        audioSource.Play();


    }
    public void ProjectileHit()
    {
        target.health.TakeDamage(CalculationManager.damage);
        turnSystem.SelectNextUnit();
        unit.isShooting = false;
        unit.actionPoints.SubtractAllActions();
    }
}
