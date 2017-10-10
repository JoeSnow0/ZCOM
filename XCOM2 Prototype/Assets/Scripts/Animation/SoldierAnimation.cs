using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAnimation : MonoBehaviour {
    Animator soldierAnimator;
    BaseUnit baseUnit;
    Vector3 lastPosition;
    Vector3 direction;
    Quaternion lookRotation;

	void Start () {
        soldierAnimator = GetComponent<Animator>();
        baseUnit = GetComponentInParent<BaseUnit>();

	}
	
	void Update () {
        if (!baseUnit.isMoving)
        {
            soldierAnimator.SetInteger("state", 0);
        }
        if (baseUnit.isMoving)
        {
            soldierAnimator.SetInteger("state", 1);
        }
        if (baseUnit.isSprinting)
        {
            soldierAnimator.SetInteger("state", 2);
        }
        if(soldierAnimator.GetInteger("state") > 0)
        {
            direction = transform.root.position - lastPosition;
            lastPosition = transform.root.position;
            lookRotation = Quaternion.LookRotation(direction);
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10);
        }
        else
        {
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10);
        }
    }
}
