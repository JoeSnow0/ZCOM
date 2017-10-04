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

    //Multiple 
        //Simple display (use if only single dices are used)
        //[Tooltip("Damage", (baseDamage+1), " + ", (baseDamage + numberOfSideDamage))]
        //DnD display (use if multiple dices are used)          Damage 2+2D4
        //[Tooltip("Damage",baseDamage," + ",numberOfDiceDamage,"D",numberOfSideDamage)]
        //Simplified display (use if multiple dices are used)   Damage 4-10
        //[Tooltip("Damage", (baseDamage + numberOfDiceDamage),"-",(baseDamage + (numberOfDiceDamage * numberOfSideDamage)))]

}



