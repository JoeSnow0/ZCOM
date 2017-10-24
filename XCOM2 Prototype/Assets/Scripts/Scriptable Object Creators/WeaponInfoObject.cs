using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]

[CreateAssetMenu(fileName = "WeaponX", menuName = "Class/CreateWeapon", order = 2)]
public class WeaponInfoObject : ScriptableObject
{


    public new string name;

    public Sprite icon;

    public int value;
    public int baseAccuracy;
    public int rangeFalloff;
    public int weaponRange;
    public int baseDamage;
    public int numberOfDiceDamage;
    public int numberOfSideDamage;

}





