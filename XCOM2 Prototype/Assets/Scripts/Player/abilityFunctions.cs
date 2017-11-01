using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abilityFunctions : MonoBehaviour {

    public MapConfig mapConfig;
    bool confirmAbility = false;
    int previousAbility = -1;

    private void Start()
    {
        //Add the map
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
    }

    public bool ConfirmAbilityCheck(int abilityIndex)
    {
        //mapConfig.turnSystem.ConfirmAbilityUse = false;
        if (previousAbility == abilityIndex)
        {
            // Do ability
            
            //use ability on target
            confirmAbility = false;
            previousAbility = -1;

            return true;
        }
        if (previousAbility == abilityIndex)
        {
            print("Doing ability-stuff()");
            //Wait for confirmation
            //make it possible to target dudes
            print("You can target stuff now");
            mapConfig.turnSystem.ConfirmAbilityUse = true;
        }
        previousAbility = abilityIndex;
        return false;
    }

    public void ShootTarget()
    {
        if (ConfirmAbilityCheck(0) == true)
        {
                Debug.Log("Pewpewpew");
            //ability happens
        }
        //Ability does not happen
        
    }
    public void Overwatch()
    {
        ConfirmAbilityCheck(1);
        Debug.Log("Unit in overwatch");
    }
    public void HunkerDown()
    {
        ConfirmAbilityCheck(2);
        Debug.Log("Hunkered down, defense increased");
    }
    public void ReloadWeapon()
    {
        ConfirmAbilityCheck(3);
        Debug.Log("Weapon reloaded");
    }
}
