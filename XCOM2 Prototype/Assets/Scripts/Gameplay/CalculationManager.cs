using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationManager : MonoBehaviour
{


    int hitRoll;
    int hitChance;


    int n;


    void HitCheck ()
    {
        //coverIntervenience = 0,25,50
        //heightAdvantage = 25

        //hitChance= (coverIntervenience + weaponBaseAccuracy + WeaponProficiency + heightAdvantage + Bonuses - Penalties - (distance * weapontypeRange))
        //hitRoll;
        hitRoll = GetComponent<RandomRange>().RollDice(1, 100);
        if (hitRoll <= hitChance)
        {
            //Run damage script
            DamageDealt();
        }
        else
        {
            //Return "Missed" Message
        }



    }


    public int DamageDealt ()
    {
        //Get Weapon damage parameters


        n = GetComponent<RandomRange>().RollDice(BaseWeapon.numberOfDiceDamage, BaseWeapon.numberOfSideDamage);

        //Give Damage dealt
        return (n);
	}




}
