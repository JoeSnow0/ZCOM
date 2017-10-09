using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{


    [SerializeField] public int value = 10;
    [SerializeField] public int baseAccuracy = 70;
    [SerializeField] public int rangeFalloff = 1;
    [SerializeField] public int weaponRange = 10;
    [SerializeField] public int baseDamage = 2;
    [SerializeField] public int numberOfDiceDamage = 1;
    [SerializeField] public int numberOfSideDamage = 3;


}



