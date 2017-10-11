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

    
    
    public static void HitCheck (WeaponInfoObject usedWeapon)
    {



        //hitChance= (coverIntervenience + weaponBaseAccuracy + WeaponProficiency + heightAdvantage + Bonuses - Penalties - (distance * weapontypeRange))
        
        hitChance = usedWeapon.baseAccuracy;

        hitRoll = RandomRange.RollDice(1, 100);
        if (hitRoll <= hitChance)
        {
            //Run damage script
            DamageDealt(usedWeapon.baseDamage, usedWeapon.numberOfDiceDamage, usedWeapon.numberOfSideDamage, true);
        }
        else
        {
            //Run damage script
            DamageDealt(0, 0, 0, false);
        }



    }


    public static void DamageDealt(int baseDamage, int numberOfDiceDamage, int numberOfSideDamage, bool contact)
    {
        int n = 0;
        //Get Weapon damage parameters


        n = RandomRange.RollDice(numberOfDiceDamage, numberOfSideDamage);

        //Give Damage dealt
        damage = n + baseDamage;
        hit = contact;
	}




}
