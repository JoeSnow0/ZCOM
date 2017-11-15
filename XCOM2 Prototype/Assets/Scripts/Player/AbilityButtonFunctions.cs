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
        //failsafes
        if (mapConfig.turnSystem.enemyUnits == null || TurnSystem.selectedUnit == null)
        {
            return;
        }

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
            //Enter shooting mode
            mapConfig.stateController.SetCurrentState(StateController.GameState.AttackMode);
            TurnSystem.EnemyTargeting = true;
            //Select first enemy unit
            mapConfig.turnSystem.KeyboardSelect(true, mapConfig.turnSystem.enemyUnits, TurnSystem.selectedTarget);
            return;
        }

    }

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
