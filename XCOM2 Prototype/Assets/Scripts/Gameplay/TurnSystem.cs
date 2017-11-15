using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(EnemySpawn))]
public class TurnSystem : MonoBehaviour
{
    [Header("Lists with all units")]
    [HideInInspector]
    public UnitConfig[] allUnits;
    [HideInInspector] public List<UnitConfig> playerUnits = new List<UnitConfig>();
    [HideInInspector] public List<UnitConfig> enemyUnits = new List<UnitConfig>();
    [System.Serializable]
    public class SpawnSetup
    {
        public UnitConfig[] enemyPrefab;
        public int spawnNumberOfEnemys;
        [HideInInspector]
        public int activatTurn;
    }
    //[Header("Actions")]
    [HideInInspector]
    static public int totalActions;
    public UnitConfig[] enemyPrefab;
    [Header("UI elements")]
    public GameObject gameOver;
    public Text gameOverText;
    public HUD hud;
    public GameObject unitInfoHolder;
    public Image classIcon;
    public Text className;
    public Text unitName;
    Animator classInformationAnimator;

    [Header("Colors")]
    public Color defeatColor;
    public Color victoryColor;
    public Color[] lineColors;
    public Gradient gradient;
    [Header("Markers")]
    public Transform cursorMarker;
    public Transform unitMarker;
    public Animator cursorAnimator;
    public Animator unitMarkerAnimator;
    public Image[] markerImage;
    [Header("Selected Unit")]
    static public UnitConfig selectedUnit;
    static public UnitConfig selectedTarget;
    public MapConfig mapConfig;
    public GenerateAbilityButtons generateAbilityButtons;
    //Enemy to spawn, can be changed to an array to randomize
    public GameObject EnemyUnitSpawnType;


    //Script refs
    public EnemySpawn enemySpawn;
    public CameraControl cameraControl;
    //public UnitConfig unitConfig;

    public bool playerTurn = true;
    public bool endTurn = false;
    public int maxTurns;
    int thisTurn = 1;
    public int enemyIndex = 0;
    //public int[] spawnEnemyTurns; old
    public SpawnSetup[] spawnSetup;

    static public bool EnemyTargeting = false;
    //Input
    public KeyCode nextTarget;
    public KeyCode previousTarget;
    public KeyCode leaveAttackMode;


    //Distance Variable (maybe put elsewhere?)
    public float distance;

    private UnitConfig lastSelectedUnit;
    public int killCount = 0;

    void Start()
    {
        //Clear lists
        playerUnits.Clear();
        enemyUnits.Clear();

        mapConfig = FindObjectOfType<MapConfig>();
        mapConfig.tileMap.Initialize();
        generateAbilityButtons = FindObjectOfType<GenerateAbilityButtons>();
        enemySpawn = GetComponent<EnemySpawn>();
        allUnits = FindObjectsOfType<UnitConfig>();

        classInformationAnimator = classIcon.transform.GetComponentInParent<Animator>();
        AddUnitsToLists();
        //Calculate total amount of action points

        cursorAnimator = cursorMarker.GetComponent<Animator>();
        unitMarkerAnimator = unitMarker.GetComponent<Animator>();




        //replenish actions to player units
        ResetActions(playerTurn);
        //Set unwalkable on unit tiles
        UpdateAllUnitsPositions();


        int loopnumber = 0;
        foreach (SpawnSetup setup in spawnSetup)
        {
            setup.activatTurn = loopnumber;
            loopnumber++;
        }
        if (playerUnits.Count > 0)
            spawnEnemy();
    }
    void Update()
    {
        UpdateHUD();

        //Deselect units on enemy turn
        if (!playerTurn && selectedUnit != null)
        {
            DeselectAllUnits();
        }

        if (playerTurn && mapConfig.stateController.CheckCurrentState(StateController.GameState.TacticalMode) && EnemyTargeting == false)
        {

            //Select next unit
            if (Input.GetKeyDown(nextTarget))
            {
                KeyboardSelect(true, playerUnits, selectedUnit);
            }
            //Select previous unit
            else if (Input.GetKeyDown(previousTarget))
            {
                KeyboardSelect(false, playerUnits, selectedUnit);
            }
        }

      

        if (playerTurn && mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
        {
            //Select next enemy unit
            if (Input.GetKeyDown(nextTarget))
            {
                if (EnemyTargeting)
                {
                    KeyboardSelect(true, enemyUnits, selectedTarget);
                }
                else
                {
                    KeyboardSelect(true, playerUnits, selectedTarget);
                }

            }
            //Select previous enemy unit
            if (Input.GetKeyDown(previousTarget))
            {
                if (EnemyTargeting)
                {
                    KeyboardSelect(false, enemyUnits, selectedTarget);
                }
                else
                {
                    KeyboardSelect(false, playerUnits, selectedTarget);
                }
            }
        }
        //Use mouse to target player units
        if (Input.GetMouseButtonDown(0) && playerTurn)
        {
            MouseSelect();
        }

        if (!playerTurn)//enemy turn
        {
            bool endturn = true;
            foreach (var enemy in enemyUnits)
            {
                if (enemy.actionPoints.CheckAvailableActions(1) || enemy.CheckUnitState(UnitConfig.UnitState.Walking))
                {
                    endturn = false;
                    break;
                }
            }
            if (endturn == true)
            {
                foreach (UnitConfig enemy in enemyUnits)
                {
                    enemy.enemyAi.isMyTurn = false;
                    enemy.currentPath = null;
                }
                hud.pressEnd(true);
                if (selectedUnit != null)
                    cameraControl.MoveToTarget(selectedUnit.transform.position);
            }
        }
        if (playerTurn)
        {
            bool endturn = true;
            foreach (UnitConfig unit in playerUnits)
            {
                if (unit.actionPoints.CheckAvailableActions(1) || unit.CheckUnitState(UnitConfig.UnitState.Walking))
                {
                    endturn = false;
                    break;
                }
            }
            if (endturn == true)
            {
                if (selectedUnit != null)
                {
                    selectedUnit.isSelected = false;
                    mapConfig.tileMap.ResetColorGrid();
                }
                selectedUnit = null;
                hud.pressEnd(true);
            }
        }

    }

    private void LateUpdate()
    {
        if (Input.GetKeyUp(leaveAttackMode))
        {
            if (mapConfig.stateController.CheckCurrentState(StateController.GameState.AttackMode))
            {
                mapConfig.stateController.SetCurrentState(StateController.GameState.TacticalMode);
                SelectUnit(selectedUnit);
                EnemyTargeting = false;
            }
        }
    }
    //public void SelectUnit()
    //{
    //    selectedUnit.isSelected = true;
    //}

    private void AddUnitsToLists()
        {
        //add units to seperate lists
        for (int i = 0; i<allUnits.Length; i++)
        {
            if (allUnits[i] != null)
            {
                if (allUnits[i].isFriendly)
                {
                    playerUnits.Add(allUnits[i]);
                }
                else
                {
                    enemyUnits.Add(allUnits[i]);
                }
            }
        }
        }
    public void DeselectUnit(UnitConfig selection)
    {
        mapConfig.tileMap.ResetColorGrid();
        selection.isSelected = false;
        selection = null;
    }
    public void DeselectAllUnits()
    {
        mapConfig.tileMap.ResetColorGrid();
        if (selectedUnit != null)
        {
            selectedUnit.isSelected = false;
            selectedUnit = null;
        }

        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].isSelected = false;
            selectedUnit = null;
        }

        if (selectedTarget != null)
        {
            selectedTarget.isSelected = false;
            selectedTarget = null;
        }

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].isSelected = false;
            selectedTarget = null;
        }

    }

    public void MouseSelect()
    {
        if ( selectedUnit != null && !selectedUnit.CheckUnitState(UnitConfig.UnitState.Idle)) 
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Use mouse to target check if its friendly
            if (!hit.collider.CompareTag("FriendlyUnit"))
            {
                return;
            }
            if (selectedUnit != null)
            {
                //Deselect previous unit
                DeselectUnit(selectedUnit);
            }
            //select new unit
            selectedUnit = hit.collider.GetComponent<UnitConfig>();
            //prevents you from targeting units without actions
            if (selectedUnit.actionPoints.CheckAvailableActions(1))
            {
                SelectUnit(selectedUnit);
            }
        }
    }
    public void SelectUnit(UnitConfig selected)
    {
        if (selected == null)
        {
            return;
        }
        if (selected.isFriendly)
        {
            selectedUnit = selected;
            //selected = selectedUnit;
            selectedUnit.isSelected = true;
            UpdateHUD();
            //Move the marker to selected unit
            MoveMarker(unitMarker, selectedUnit.transform.position);
            //Update grid colors
            mapConfig.tileMap.ChangeGridColor(selected.movePoints, selected.actionPoints.ReturnAvailableActions(), selected);
            //Update Displayed Name
            unitName.text = selectedUnit.unitName;
            className.text = selectedUnit.unitClassStats.unitClassName;
            //Clear old abilities
            generateAbilityButtons.ClearCurrentButtons();
            //Generate new abilities buttons if its a player unit
            generateAbilityButtons.GenerateCurrentButtons(selectedUnit.unitAbilities);
            //Move the camera to selected Unit/target
            cameraControl.MoveToTarget(selectedUnit.transform.position);
        }
        if (!selected.isFriendly)
        {
            //HACK: If you're targeting an enemy, what do?
            selectedTarget = selected;
            //Move the camera to selected Unit/target
            cameraControl.MoveToTarget(selectedTarget.transform.position);
        }
    }

    //Update the tile that need to be unwalkable for a specific unit
    public void UpdateUnitPosition(UnitConfig unit)
    {
        mapConfig.tileMap.UnitMapData(unit.tileX, unit.tileY);
    }

    //Update the tiles that need to be unwalkable for all units
    public void UpdateAllUnitsPositions()
    {
        foreach (var unit in allUnits)
        {
            mapConfig.tileMap.UnitMapData(unit.tileX, unit.tileY);
        }
    }

    //Cycle through list of targets
    public void KeyboardSelect(bool ChooseNext, List<UnitConfig> unitList, UnitConfig selected)
    {
        //if list is empty, exit function
        if (unitList == null)
        {
            return;
        }
        //If selected is null, pick the first unit in the list
        if (selected == null)
        {
            selected = unitList[0];
        }
        //Check if unit is idle
        if (!selected.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            return;
        }
        int chosenUnitIndex = unitList.FindIndex(a => a == selected);
        DeselectUnit(selected);
        
        if (selected.isFriendly)
        {
            //Check if any friendly units have actions left
            //Makes sure you can't target anything at the end of your turn
            bool UnitHasActionsLeft = false;
            foreach (UnitConfig unit in unitList)
            {
                if (unit.actionPoints.CheckAvailableActions(1))
                {
                    UnitHasActionsLeft = true;
                    break;
                }
            }
            if (UnitHasActionsLeft == false)
            {
                return;
            }
        }
        //After all the checks are done, we can start choosing a target
        for (int i = 0; i < unitList.Count; i++)
        {
            //loops around the list
            //Go up the list
            if (ChooseNext)
            {
                chosenUnitIndex++;
            }
            //Go down the list
            else if (!ChooseNext)
            {
                chosenUnitIndex--;
            }
            //If its too high, loop around
            if (chosenUnitIndex >= unitList.Count)
            {
                chosenUnitIndex = 0;
            }
            //If its too low, loop around
            else if (chosenUnitIndex < 0)
            {
                chosenUnitIndex = unitList.Count - 1;
            }

            //Check if enemy unit is targetable
            if (!unitList[chosenUnitIndex].isFriendly)
            {
                selected = unitList[chosenUnitIndex];
                selectedTarget = selected;
                selectedUnit.GetAccuracy(selectedTarget.tileX, selectedTarget.tileY);
                generateAbilityButtons.abilityChanceToHit.text = "Chance to hit: " + UnitConfig.accuracy +"%";
                break;
            }
            if (unitList[chosenUnitIndex].isFriendly && unitList[chosenUnitIndex].actionPoints.CheckAvailableActions(1))
            {
                selected = unitList[chosenUnitIndex];
                selectedUnit = selected;
                break;
            }

        }

        //Select the next/previous unit
        SelectUnit(selected);

    }

    public void MoveMarker(Transform marker, Vector3 newPosition)
    {
        marker.position = newPosition;
    }
    public void ToggleMarkers(bool display)
    {
        cursorAnimator.SetBool("display", display);
        unitMarkerAnimator.SetBool("display", display);
    }

    public void ResetActions(bool isPlayerTurn)
    {
        //replenishes the actions of player/enemy and resets the total actions variable.
        totalActions = 0;
        //For player units
        if (isPlayerTurn)
        {
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].actionPoints.ReplenishAllActions();

            }
        }
        //For enemy units
        else
        {
            if (enemyUnits == null)
            {
                return;
            }

            else
            {
                for (int i = 0; i < enemyUnits.Count; i++)
                {
                    enemyUnits[i].actionPoints.ReplenishAllActions();
                }
            }
        }
        
        playerTurn = isPlayerTurn;
    }

    public void StartNextEnemy()
    {
        if (enemyUnits == null ||
            enemyUnits[enemyIndex] == null ||
            enemyUnits[enemyIndex].enemyAi == null ||
            enemyUnits.Count < 1)
        {
        }

        enemyUnits[enemyIndex].enemyAi.isMyTurn = true;
        enemyIndex++;
    }
    public int getCurrentTurn(int currentTurn)
    {
        if (currentTurn > maxTurns)
        {
            //deactivates the map
            gameObject.SetActive(false);
            gameOver.SetActive(true);
        }
        thisTurn = currentTurn;
        return maxTurns;
    }

    public void destroyUnit(UnitConfig unit)
    {
        if (unit.isFriendly)
        {
            unit.Die();//Animate death
            playerUnits.Remove(unit);
        }

        else
        {
            unit.Die();//Animate death
            enemyUnits.Remove(unit);
        }

        if (playerUnits.Count <= 0)
        {
            gameObject.SetActive(false);//deactivates the map
            gameOver.SetActive(true);
            gameOverText.text = "DEFEAT";
            gameOverText.color = defeatColor;
        }
    }
    public void spawnEnemy()
    {

        foreach (SpawnSetup i in spawnSetup) // Checks if current turn should spawn an enemy
        {
            if (i.activatTurn == thisTurn)
            {
                enemySpawn.SpawnEnemy(i, i.spawnNumberOfEnemys);
                break;
            }
            else if (spawnSetup.Length <= thisTurn)
            {
                int number = Random.Range(0, spawnSetup.Length);
                enemySpawn.SpawnEnemy(spawnSetup[number], spawnSetup[number].spawnNumberOfEnemys);
                break;
            }
        }
        //int newI = Random.Range(0, spawnSetup.Length);
        //enemySpawn.SpawnEnemy(spawnSetup[newI].enemyPrefab, spawnSetup[newI].spawnNumberOfEnemys);
    }

    private void UpdateHUD()
    {
        unitInfoHolder.SetActive(playerTurn);

        if (selectedUnit != null && selectedUnit != lastSelectedUnit)
        {
            classInformationAnimator.Play("UnitInfoTransition", -1, 0f);
            className.text = selectedUnit.unitClassStats.unitClassName;
            unitName.text = selectedUnit.unitName;
            classIcon.sprite = selectedUnit.unitClassStats.classIcon;
        }

        foreach (UnitConfig unit in playerUnits)//Updates friendly units
        {
            if (unit.isSelected || unit.isHighlighted)
            {
                unit.animatorHealthbar.SetBool("display", true);
            }
            else
            {
                unit.animatorHealthbar.SetBool("display", false);
            }
        }

        foreach (UnitConfig unit in enemyUnits) //Update enemy units
        {
            if (!playerTurn && unit.enemyAi.isMyTurn || unit.isHighlighted || selectedUnit != null && selectedUnit.animator.target != null && selectedUnit.animator.target == unit /*|| unit.enemyAi.isHighlighted   CODE FOR IF THE UNIT IS HIGHLIGHTED     */)
            {
                unit.animatorHealthbar.SetBool("display", true);
            }
            else if (unit.animatorHealthbar != null)
            {
                unit.animatorHealthbar.SetBool("display", false);
            }
        }
    }
}
