using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationManager : MonoBehaviour
{


    int hitRoll;
    int hitChance;
    [SerializeField] public int smallCoverIntervenience = 25;
    [SerializeField] public int largeCoverIntervenience = 50;
    [SerializeField] public int heightAdvantage = 25;


    int n;


    void HitCheck ()
    {
        


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

        //int Baseweapon = GetComponent(numberOfDiceDamage);
        //n = GetComponent<RandomRange>().RollDice(numberOfDiceDamage, BaseWeapon.numberOfSideDamage);

        //Give Damage dealt
        return (n);
	}




}
