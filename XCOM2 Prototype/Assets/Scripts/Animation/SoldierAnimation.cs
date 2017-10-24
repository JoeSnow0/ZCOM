using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAnimation : MonoBehaviour {
    Animator soldierAnimator;
    UnitConfig Unit;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;

	void Start () {
        soldierAnimator = GetComponent<Animator>();
        Unit = GetComponentInParent<UnitConfig>();

	}
	
	void Update () {
        if (!Unit.isMoving)
        {
            soldierAnimator.SetInteger("state", 0);
        }
        if (Unit.isMoving)
        {
            soldierAnimator.SetInteger("state", 1);
        }
        if (Unit.isSprinting)
        {
            soldierAnimator.SetInteger("state", 2);
        }
        if(soldierAnimator.GetInteger("state") > 0) // HACK: What!?
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
