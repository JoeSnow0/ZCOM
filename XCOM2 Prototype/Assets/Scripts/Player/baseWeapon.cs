using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseWeapon : MonoBehaviour {

    [Header("Weapon Statistics")]
    [Tooltip("Accuracy")]
    [Range( 0f,  100f)]
    public int weaponAim;
    [Tooltip("Damage")]
    [Range(1f, 5f)]
    public int weaponDamage;
}
