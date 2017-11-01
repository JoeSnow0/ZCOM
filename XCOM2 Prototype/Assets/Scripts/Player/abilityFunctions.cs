using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abilityFunctions : MonoBehaviour {

    public MapConfig mapConfig;
    public TextMesh infoText;
    //bool confirmAbility = false;
    int previousAbility = -1;

    private void Awake()
    {
        //Add the map
        
        //Get UI Element for ability text
    }
    
    public bool ConfirmAbilityCheck(int abilityIndex)
    {
        if (infoText == null || mapConfig == null)
        {
            previousAbility = -1;
            infoText = GameObject.FindGameObjectWithTag("UI_Elements").GetComponentInChildren<TextMesh>();
            mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        }
        infoText.text = "Confirm Ability Use";
        //make it possible to target dudes
        mapConfig.turnSystem.EnemyTargeting = true;
        print("You can target stuff now");

        if (previousAbility == abilityIndex)
        {
            print("Doing ability-stuff()");
            return true;
        }
        previousAbility = abilityIndex;
        return false;
    }

    public void ShootTarget()
    {
        //if ability has been confirmed by player
        if (ConfirmAbilityCheck(0))
        {
            //ability happens
            print("Pewpewpew");
            //stop targeting mode
            mapConfig.turnSystem.DeselectAllUnits();
            return;
        }
        //Ability does not happen
        print("no pewpew");
        mapConfig.turnSystem.EnemyTargeting = false;
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
