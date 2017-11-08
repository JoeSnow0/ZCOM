using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

//Creates a scriptable object with the parameters below
[CreateAssetMenu(fileName = "WeaponX", menuName = "Class/CreateWeapon", order = 2)]
public class WeaponInfoObject : ScriptableObject
{
    string damageString;

    [Header("Name of Weapon")]
    public new string name;

    [Header("Icon of Weapon")]
    public Sprite icon;

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Cost")]
    [Tooltip("The value of an item if it were bought or sold")]
    [RangeAttribute(0, 100)]
    public int value;

    [Header("Aim")]
    [Tooltip("The amount of Aim the gun has by default")]
    [RangeAttribute(0, 100)]
    public int baseAim;

    //Use these to calculate hit chance for each level of distance
    [Header("Range Modifiers")]
    [Header("Short Distance")]
    [RangeAttribute(-100, 100)]
    public int rangeModShort;

    [Header("Medium Distance")]
    [RangeAttribute(-100, 100)]
    public int rangeModMedium;

    [Header("Long Distance")]
    [RangeAttribute(-100, 100)]
    public int rangeModLong;

    [Header("Far Distance")]
    [RangeAttribute(-100, 100)]
    public int rangeModFar;
    //Limit the range from 0-4, where 0 = melee only, 4 = melee -> far range
    [Header("Max Range")]
    [Tooltip("0 = melee only, 4 = maximum range")]
    [RangeAttribute(0, 4)]
    public int weaponRange;

    [Header("Base Damage")]
    [Tooltip("base damage")]
    [RangeAttribute(0, 10)]
    public int baseDamage;

    [Header("Extra Damage")]
    [Tooltip("This is added on to the base damage")]
    public int numberOfDiceDamage;
    public int numberOfSidesDamage;

    [Header("Customization")]
    [Tooltip("Projectile particle system used when the weapon is shooting")]
    public GameObject weaponProjectile;
    [Tooltip("Random color between the chosen")]
    public Color[] particleColor;
    public AudioClip weaponSoundShoot;
    public AudioClip weaponSoundReload;
    
    //HACK: Can I do a calculation in a scriptable object?
    //int minDamage = baseDamage + (numberOfDiceDamage * 1);
    //int maxDamage = baseDamage + (numberOfDiceDamage * numberOfSidesDamage);
}





