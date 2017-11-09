using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class abilityFunctions : MonoBehaviour {

    private MapConfig _mapConfig = null;
    public MapConfig mapConfig { get { if (_mapConfig == null) _mapConfig = GameObject.FindObjectOfType<MapConfig>(); return _mapConfig; } }

    public Text abilityTooltip;
    //
    public enum AbilityStuff
    {
        None = 0,
        Shoot,
        Overwatch,
        Hunker,
        Reload,
        Medkit
    }

    static public AbilityStuff previousAbility = AbilityStuff.None;

    private void Start()
    {
        //Add the map incase its missing
        //mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        previousAbility = AbilityStuff.None;
        _mapConfig = null;
    }

    private void Awake()
    {
        previousAbility = AbilityStuff.None;
        //mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        _mapConfig = null;
    }

    public bool ConfirmAbilityCheck(AbilityStuff ability)
    {
        if (previousAbility == ability)
        {
            previousAbility = AbilityStuff.None;
            return true;
        }
        previousAbility = ability;
        return false;
    }

    public bool ConfirmAbilityCheck(int abilityIndex)
    {
        //make it possible to target with attack mode
        return ConfirmAbilityCheck((AbilityStuff)abilityIndex);           
    }

    public void ShootTarget()
    {
        //if ability has been confirmed by player
        if (ConfirmAbilityCheck(AbilityStuff.Shoot) && mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
        {
            //Shooting target
            if (!TurnSystem.selectedTarget.isFriendly) //Checks if the unit hit is not friendly
            {
                //Spend Actions
                mapConfig.turnSystem.totalActions -= TurnSystem.selectedUnit.actionPoints.actions;
                TurnSystem.selectedUnit.actionPoints.SubtractAllActions();

                //Calculate the distance between the units
                mapConfig.turnSystem.distance = Vector3.Distance(TurnSystem.selectedUnit.transform.position, TurnSystem.selectedTarget.transform.position);
                //Uses current weapon
                CalculationManager.HitCheck(TurnSystem.selectedUnit.unitWeapon, mapConfig.turnSystem.distance);
                TurnSystem.selectedUnit.ShootTarget(TurnSystem.selectedTarget);

                //Calculate the distance between the units
                mapConfig.turnSystem.distance = Vector3.Distance(TurnSystem.selectedUnit.transform.position, TurnSystem.selectedTarget.transform.position);
                mapConfig.turnSystem.distance /= 2;

            }
            mapConfig.stateController.SetCurrentState(StateController.GameState.TacticalMode);
            //stop targeting mode
            mapConfig.turnSystem.DeselectAllUnits();
        }
        else
        {
            //Ability does not happen
            print("pls confirm");
            mapConfig.turnSystem.KeyboardSelect(true, mapConfig.turnSystem.enemyUnits, TurnSystem.selectedTarget);
            mapConfig.stateController.SetCurrentState(StateController.GameState.AttackMode);
        }
       
    }

    /// <summary>
    /// ///// ignore below
    /// </summary>

    public void Overwatch()
    {
        ConfirmAbilityCheck(AbilityStuff.Overwatch);
        Debug.Log("Unit in overwatch");
    }
    public void HunkerDown()
    {
        ConfirmAbilityCheck(AbilityStuff.Hunker);
        Debug.Log("Hunkered down, defense increased");
    }
    public void ReloadWeapon()
    {
        ConfirmAbilityCheck(AbilityStuff.Reload);
        Debug.Log("Weapon reloaded");
    }
    public void Medkit()
    {
        ConfirmAbilityCheck(AbilityStuff.Medkit);
        Debug.Log("Healing");
    }
}
