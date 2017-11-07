using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class abilityFunctions : MonoBehaviour {

    public MapConfig mapConfig;
    public Text abilityTooltip;
    int previousAbility = -1;

    private void Start()
    {
        //Add the map incase its missing
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
    }

    public bool ConfirmAbilityCheck(int abilityIndex)
    {
        //Add the map incase its missing
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        //make it possible to target dudes
        mapConfig.stateController.SetCurrentState(StateController.GameState.AttackMode);
        print(mapConfig.stateController.CurrentState.ToString());

        if (previousAbility == abilityIndex)
        {
            print("Doing ability-stuff()");
            return true;
        }
        previousAbility = abilityIndex;
        mapConfig.stateController.SetCurrentState(StateController.GameState.TacticalMode);
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
            mapConfig.stateController.SetCurrentState(StateController.GameState.TacticalMode);
            return;
        }
        //Ability does not happen
        print("pls confirm");
    }

    /// <summary>
    /// ///// ignore below
    /// </summary>

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
