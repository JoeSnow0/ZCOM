using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimation : MonoBehaviour {
    Animator zombieAnimation;
    UnitConfig Unit;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;

	void Start () {
        zombieAnimation = GetComponent<Animator>();
        Unit = GetComponentInParent<UnitConfig>();

	}
	
	void Update () {
        if (!Unit.isMoving)
        {
            zombieAnimation.SetInteger("state", 0);
        }
        if (Unit.isMoving)
        {
            zombieAnimation.SetInteger("state", 1);
        }
        if (Unit.isSprinting)
        {
            zombieAnimation.SetInteger("state", 2);
        }
        if(zombieAnimation.GetInteger("state") > 0) // HACK: What!?
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
}
