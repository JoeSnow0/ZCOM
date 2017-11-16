using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbilityButtonFunctions : MonoBehaviour
{
    public KeyCode abilityKeybind;
    public Button abilityButton;
    private MapConfig _mapConfig = null;
    public MapConfig mapConfig { get { if (_mapConfig == null) _mapConfig = GameObject.FindObjectOfType<MapConfig>(); return _mapConfig; } }

    public Text abilityTooltip;
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
        abilityButton = GetComponent<Button>();
        previousAbility = AbilityStuff.None;
        _mapConfig = null;
    }

    private void Awake()
    {
        previousAbility = AbilityStuff.None;
        _mapConfig = null;
    }

    public void Update()
    {
        if (Input.GetKeyDown(abilityKeybind))
        {
            abilityButton.onClick.Invoke();
        }
    }

    public bool ConfirmAbilityCheck(AbilityStuff ability)
    {
        mapConfig.turnSystem.cameraControl.MoveToTarget(TurnSystem.selectedUnit.transform.position);
        if (previousAbility == ability)
        {
            previousAbility = AbilityStuff.None;
            return true;
        }
        previousAbility = ability;
        return false;
    }

    //public bool ConfirmAbilityCheck(int abilityIndex)
    //{
    //    //make it possible to target with attack mode
    //    return ConfirmAbilityCheck((AbilityStuff)abilityIndex);           
    //}

    public void ShootTarget()
    {
        //failsafes, need to be added to all abilities
        if (!TurnSystem.selectedUnit.CheckUnitState(UnitConfig.UnitState.Idle) || !TurnSystem.selectedUnit.actionPoints.CheckAvailableActions(1))
        {
            return;
        }
        if (mapConfig.turnSystem.enemyUnits == null || TurnSystem.selectedUnit == null)
        {
            return;
        }

        //Actual ability start
        //Check for confirmation & make sure you're in the correct mode
        if (ConfirmAbilityCheck(AbilityStuff.Shoot) && mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
        {
            //Trigger Ranged Attack
            TurnSystem.selectedUnit.RangedAttack(TurnSystem.selectedUnit, TurnSystem.selectedTarget);
            //End shooting mode
            mapConfig.stateController.SetCurrentState(StateController.GameState.TacticalMode);
            //select next unit
            mapConfig.turnSystem.KeyboardSelect(true, mapConfig.turnSystem.playerUnits, TurnSystem.selectedUnit);
            return;
        }
        else
        {
            //Select first enemy unit
            mapConfig.turnSystem.KeyboardSelect(true, mapConfig.turnSystem.enemyUnits, TurnSystem.selectedTarget);
            //Enter shooting mode
            mapConfig.stateController.SetCurrentState(StateController.GameState.AttackMode);
            TurnSystem.EnemyTargeting = true;
            if (TurnSystem.selectedTarget.CheckUnitState(UnitConfig.UnitState.Dead))
            {
                //Enter shooting mode
                mapConfig.stateController.SetCurrentState(StateController.GameState.TacticalMode);
                TurnSystem.EnemyTargeting = false;      
            }
            return;
        }

    }

    public void Overwatch()
    {
        //failsafes, need to be added to all abilities
        if (!TurnSystem.selectedUnit.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            return;
        }
        if (mapConfig.turnSystem.enemyUnits == null || TurnSystem.selectedUnit == null)
        {
            return;
        }
        //Actual ability start
        if (ConfirmAbilityCheck(AbilityStuff.Overwatch) && mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
        {
            //This happens when ability is activated
            Debug.Log("Unit in overwatch");
        }
        else
        {
            //This happens when ability is toggled (first click)
            return;
        }
    }
    public void HunkerDown()
    {
        //failsafes, need to be added to all abilities
        if (!TurnSystem.selectedUnit.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            return;
        }
        if (mapConfig.turnSystem.enemyUnits == null || TurnSystem.selectedUnit == null)
        {
            return;
        }
        //Actual ability start
        if (ConfirmAbilityCheck(AbilityStuff.Hunker) && mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
        {
            //This happens when ability is activated
            Debug.Log("Hunkered down, defense increased");
        }
        else
        {
            //This happens when ability is toggled (first click)
            return;
        }
        
    }
    public void ReloadWeapon()
    {
        //failsafes, need to be added to all abilities
        if (!TurnSystem.selectedUnit.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            return;
        }
        if (mapConfig.turnSystem.enemyUnits == null || TurnSystem.selectedUnit == null)
        {
            return;
        }
        //Actual ability start
        if (ConfirmAbilityCheck(AbilityStuff.Reload) && mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
        {
            //This happens when ability is activated
            Debug.Log("Weapon reloaded");
        }
        else
        {
            //This happens when ability is toggled (first click)
            return;
        }
    }
    public void Medkit()
    {
        //failsafes, need to be added to all abilities
        if (!TurnSystem.selectedUnit.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            return;
        }
        if (mapConfig.turnSystem.enemyUnits == null || TurnSystem.selectedUnit == null)
        {
            return;
        }
        //Actual ability start
        if (ConfirmAbilityCheck(AbilityStuff.Medkit) && mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
        {
            //This happens when ability is activated
            Debug.Log("Healing");
        }
        else
        {
            //This happens when ability is toggled (first click)
            return;
        }
    }
}
