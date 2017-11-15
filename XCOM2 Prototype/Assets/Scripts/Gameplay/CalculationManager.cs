using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CalculationManager// : MonoBehaviour
{
    
    static int hitRoll;
    static int hitChance;
    public static int smallCoverIntervenience = 25;
    public static int largeCoverIntervenience = 50;
    public static int heightAdvantage = 25;
    public static int damage = 0;
    public static bool hit;
    
    public static void HitCheck (WeaponInfoObject usedWeapon, int aim)
    {
        hitChance = aim;
        
        //hitChance= (coverIntervenience + weaponBaseAccuracy + WeaponProficiency + heightAdvantage + Bonuses - Penalties - (distance * weapontypeRange))
        hitRoll = RandomRange.RollDice(1, 100);
        if (hitRoll <= hitChance)
        {
            //Run damage script
            DamageDealt(usedWeapon.baseDamage, usedWeapon.numberOfDiceDamage, usedWeapon.numberOfSidesDamage, true);
        }
        else
        {
            //Run damage script
            DamageDealt(0, 0, 0, false);
        }
        
        //int tempCase = usedWeapon.weaponRange;
        //switch (tempCase)
        //{
        //    case 0:
        //        Debug.Log("case 1");
        //        break;
        //    case 2:
        //        Debug.Log("case 2");
        //        break;
        //    default:
        //        Debug.Log("default");
        //        break;
        //}
    }
    
    public static void DamageDealt(int baseDamage, int numberOfDiceDamage, int numberOfSideDamage, bool contact)
    {
        int n = 0;
        //Get Weapon damage parameters
        n = RandomRange.RollDice(numberOfDiceDamage, numberOfSideDamage);

        //Give Damage dealt
        hit = contact;
        damage = n + baseDamage;
        
	}
}
