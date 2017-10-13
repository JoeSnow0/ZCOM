using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "WeaponX", menuName = "Class/CreateWeapon", order = 2)]
public class WeaponInfoObject : ScriptableObject
{


    public new string name;

    public Sprite icon;
    [Header("Health")]
    [Tooltip("The amount of health this class has")]
    [RangeAttribute(1, 40)]
    public int goldValue;
    [Header("Aim")]
    [Tooltip("The amount of health this class has")]
    [RangeAttribute(0, 100)]
    public int baseAim;
    [RangeAttribute(-100, 100)]
    public int rangeModShort;
    [RangeAttribute(-100, 100)]
    public int rangeModMedium;
    [RangeAttribute(-100, 100)]
    public int rangeModLong;
    [RangeAttribute(-100, 100)]
    public int rangeModFar;
    public int weaponRange;
    public int baseDamage;
    public int numberOfDiceDamage;
    public int numberOfSideDamage;

}





