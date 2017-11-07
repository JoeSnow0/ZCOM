using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(EnemySpawn))]
public class TurnSystem : MonoBehaviour {
    [Header("Lists with all units")]
    [HideInInspector]public UnitConfig[] allUnits;
    [HideInInspector]public List<UnitConfig> playerUnits = new List<UnitConfig>();
    [HideInInspector]public List<UnitConfig> enemyUnits = new List<UnitConfig>();
    [System.Serializable]
    public class SpawnSetup
    {
        public UnitConfig enemyPrefab;
        public int spawnNumberOfEnemys;
        [HideInInspector]
        public int activatTurn;
    }
    //[Header("Actions")]
    [HideInInspector]
    public int totalActions;
    public UnitConfig[] enemyPrefab;
    [Header("UI elements")]
    public GameObject gameOver;
    public Text gameOverText;
    public HUD hud;
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
    public UnitConfig selectedUnit;
    public UnitConfig selectedTarget;
    public MapConfig mapConfig;
    public generateButtons generateButtons;
    //Enemy to spawn, can be changed to an array to randomize
    public GameObject EnemyUnitSpawnType;
    public Text className;

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
    
    public bool EnemyTargeting = false;
    //Input
    public KeyCode nextTarget;
    public KeyCode previousTarget;
    

    //Distance Variable (maybe put elsewhere?)
    public float distance;



    void Start ()
    {
        mapConfig = FindObjectOfType<MapConfig>();
        mapConfig.tileMap.Initialize();
        generateButtons = FindObjectOfType<generateButtons>();
        enemySpawn = GetComponent<EnemySpawn>();
        allUnits = FindObjectsOfType<UnitConfig>();
        
        //add units to array
        for (int i = 0; i < allUnits.Length; i++)
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
        //Calculate total amount of action points
        for (int i = 0; i < playerUnits.Count; i++)
        {   
            totalActions += playerUnits[i].actionPoints.actions;
        }
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
        if(playerUnits.Count > 0)
            spawnEnemy();
    }
	void Update () {

        attackUnit();
        UpdateHUD();

        //Deselect units on enemy turn
        if (!playerTurn && selectedUnit != null)
        {
            DeselectAllUnits();
        }
        
        if (playerTurn && mapConfig.stateController.CheckCurrentState() == StateController.GameState.TacticalMode)
        {
            //Select next unit
            if (Input.GetKeyDown(nextTarget))
            {
                SwitchTarget(true, playerUnits, selectedUnit);
            }
            //Select previous unit
            if (Input.GetKeyDown(previousTarget))
            {
                SwitchTarget(false, playerUnits, selectedUnit);
            }
        }
        if (playerTurn && mapConfig.stateController.CheckCurrentState() == StateController.GameState.AttackMode)
        {
            //Select next enemy unit
            if (Input.GetKeyDown(nextTarget))
            {
                SwitchTarget(true, enemyUnits, selectedTarget);
            }
            //Select previous enemy unit
            if (Input.GetKeyDown(previousTarget))
            {
                SwitchTarget(false, enemyUnits, selectedTarget);
            }
            
        }

        if (Input.GetMouseButtonDown(0) && playerTurn && !selectedUnit.isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.CompareTag("FriendlyUnit"))
                {
                    if (selectedUnit != null)
                    {
                        selectedUnit.isSelected = false;
                    }

                    selectedUnit = hit.collider.GetComponent<UnitConfig>();
                    //prevents you from targeting units without actions
                        if (selectedUnit.actionPoints.actions != 0)
                        {
                            SwitchTarget(true, playerUnits, selectedUnit);
                        }
                    
                    }

                }
            }

        if (!playerTurn)//enemy turn
        {
            bool endturn = true;
            foreach (var enemy in enemyUnits)
            {
                if (enemy.actionPoints.actions > 0 || enemy.isMoving)
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
                if(selectedUnit != null)
                    cameraControl.MoveToTarget(selectedUnit.transform.position);
            }
        }
        if (playerTurn)
        {
            bool endturn = true;
            foreach (UnitConfig unit in playerUnits)
            {
                if (unit.actionPoints.actions > 0 || unit.isMoving)
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
    public void DeselectUnit(UnitConfig unit)
    {
        if (unit.isFriendly)
        {
            unit.isSelected = false;
            selectedUnit = null;
        }
        if (!unit.isFriendly)
        {
            unit.isSelected = false;
            selectedTarget = null;
        }


    }
    public void DeselectAllUnits()
    {
        selectedUnit = null;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].isSelected = false;
            selectedUnit = null;
        }
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].isSelected = false;
            selectedTarget = null;
        }

    }
    //HACK: SelectedUnit is the same as SelectedUnit that is Selected? could be simplified?
    public void selectUnit(UnitConfig selected)
    {
        if (selected.isFriendly)
        {
            //mapConfig.tileMap.selected = selected;

            selected.isSelected = true;
            //Move the marker to selected unit
            MoveMarker(unitMarker, selected.transform.position);
            //Move the camera to selected Unit
            MoveCameraToTarget(selected.transform.position, 0);
            //Update grid colors
            if (playerTurn)
                mapConfig.tileMap.ChangeGridColor(selected.movePoints, selected.actionPoints.actions, selected);

            className.text = selected.unitClassStats.unitClassName;
            //HACK: Buttons are broken uncomment when fixed
            //Clear old abilities
            generateButtons.ClearCurrentButtons();
            if (selected.isFriendly == true)
            {
                //Generate new abilities buttons if its a player unit
                generateButtons.GenerateCurrentButtons(selected.unitAbilities);
            }
        }
        if (!selected.isFriendly)
        {
            //Move the camera to selected Unit
            MoveCameraToTarget(selected.transform.position, 0);
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
    public void SwitchTarget(bool nextTarget, List<UnitConfig> unitList, UnitConfig selected)
    {
        if (selectedUnit.isMoving)
        {
            return;
        }
        int currentUnitIndex;
        
        //check if list is empty
        if (unitList != null)
        {
            if (selected != null)
            {
                currentUnitIndex = unitList.FindIndex(a => a == selected);
            }
            //If its empty, pick the first friendly unit in list
            else
            {
                selected = unitList[0];
                currentUnitIndex = unitList.FindIndex(a => a == selected);
            }
            if (selected.isFriendly)
            {
                //Check if any units have actions left
                bool UnitHasActionsLeft = false;
                foreach (UnitConfig unit in unitList)
                {
                    if (unit.actionPoints.actions > 0)
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

            //move to next unit in list if true
            if (nextTarget)
            {
                for (int i = 0; i < unitList.Count; i++)
                {
                    //loops around to the beginning of the list
                    currentUnitIndex += 1;
                    if (currentUnitIndex > unitList.Count)
                    {
                        currentUnitIndex = 0;
                    }

                    selected = unitList[currentUnitIndex % unitList.Count];

                    if (selected.isFriendly)
                    {
                        if (selected.actionPoints.actions > 0)
                        {
                            break;
                        }

                    }
                    //Check if enemy unit is targetable
                    if (!selected.isFriendly)
                    {
                        break;
                    }
                }
            }

            else
            {
                //move to previous unit in list if false
                for (int i = 0; i < unitList.Count; i++)
                {

                    //loops around to the end of the list
                    currentUnitIndex -= 1;
                    if (currentUnitIndex < 0)
                    {
                        currentUnitIndex = unitList.Count - 1;
                    }
                    selected = unitList[currentUnitIndex % unitList.Count];
                    if (selected.isFriendly)
                    {
                        if (selected.actionPoints.actions > 0)
                        {
                            break;
                        }
                    }

                }
            }
            //Select the next/previous unit
            selectUnit(selected);
        }
    }
    
    //HACK: Need to move the attackUnit function to unitConfig script
    void attackUnit()
    {
        if (Input.GetMouseButtonDown(0) && playerTurn) //Checks if it is the players turn
        {
            if (selectedUnit == null)
            {
                return;
            }
            if (selectedUnit.actionPoints.actions > 0) //Checks if the unit has enough action points
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.GetComponent<UnitConfig>()) //Checks if the unit hit an enemy
                    {
                        UnitConfig target = hit.collider.GetComponent<UnitConfig>();
                        if (!target.isFriendly) //Checks if the unit hit is not friendly
                        {
                            //Spend Actions
                            totalActions -= selectedUnit.actionPoints.actions;
                            selectedUnit.actionPoints.SubtractAllActions();

                            //Calculate the distance between the units
                            distance = Vector3.Distance(selectedUnit.transform.position, target.transform.position);
                            //Uses current weapon
                            CalculationManager.HitCheck(selectedUnit.unitWeapon, distance);
                            selectedUnit.ShootTarget(target);

                            //Calculate the distance between the units
                            distance = Vector3.Distance(selectedUnit.transform.position, target.transform.position);
                            distance /= 2;

                        }
                    }
                }
            }
        }
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
                totalActions += playerUnits[i].actionPoints.actions;

            }
        }
        //For enemy units
        else
        {
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].actionPoints.ReplenishAllActions();
                totalActions += enemyUnits[i].actionPoints.actions;
            }
        }
        playerTurn = isPlayerTurn;
    }
    
    //HACK: Why so many select unit functions!?
    //public void SelectNextUnit()
    //{
    //    for(int i = 0; i < playerUnits.Count; i++)
    //    {
    //        if(playerUnits[i].actionPoints.actions > 0 && selectedUnit != playerUnits[i])
    //        {
    //            if (selectedUnit != null)
    //            {
    //                selectedUnit.isSelected = false;
    //            }
    //            selectedUnit = playerUnits[i];
    //            selectedUnit.isSelected = true;
    //            MoveMarker(unitMarker, selectedUnit.transform.position);
    //            MoveCameraToTarget(selectedUnit.transform.position, 0);
    //            if(playerTurn && selectedUnit != null)
    //                mapConfig.tileMap.ChangeGridColor(selectedUnit.movePoints, selectedUnit.actionPoints.actions, selectedUnit);
    //            break;
    //        }
    //    }
    //    /*if (selectedUnit == null && playerUnits.Count > 0)
    //    {
    //        selectedUnit = playerUnits[0];
    //        selectedUnit.isSelected = true;
    //    }*/
    //    if(selectedUnit != null)
    //        className.text = selectedUnit.unitClassStats.unitClassName;
        
    //}
    public void StartNextEnemy()
    {
        if (enemyUnits == null ||
            enemyUnits[enemyIndex] == null ||
            enemyUnits[enemyIndex].enemyAi == null || 
            enemyUnits.Count < 1)
        {
            Debug.Break();
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
            

        //Destroy(unit.gameObject);
        //if(enemyUnits.Count <= 0)
        //{
        //    gameOver.SetActive(true);
        //    gameOverText.text = "VICTORY";
        //    gameOverText.color = victoryColor;
        //}
        if(playerUnits.Count <= 0)
        {
            gameObject.SetActive(false);//deactivates the map
            gameOver.SetActive(true);
            gameOverText.text = "DEFEAT";
            gameOverText.color = defeatColor;
        }
    }
    public void spawnEnemy()
    {
        
        //foreach (SpawnSetup i in spawnSetup) // Checks if current turn should spawn an enemy
        //{
        //    if(i.activatTurn == thisTurn)
        //    {
        //        enemySpawn.SpawnEnemy(i.enemyPrefab,i.spawnNumberOfEnemys);
        //        break;
        //    }
        //    else if (spawnSetup.Length <= thisTurn)
        //    {
        //        int newI = Random.Range(0, spawnSetup.Length);
        //        enemySpawn.SpawnEnemy(spawnSetup[newI].enemyPrefab,spawnSetup[newI].spawnNumberOfEnemys);
        //        break;
        //    }
        //}
        int newI = Random.Range(0, spawnSetup.Length);
        enemySpawn.SpawnEnemy(spawnSetup[newI].enemyPrefab, spawnSetup[newI].spawnNumberOfEnemys);
    }

    private void UpdateHUD()
    {
        foreach (UnitConfig unit in playerUnits)//Updates friendly units
        {
            if (unit.isSelected)
            {
                foreach (Image image in unit.imageElements.elements)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, unit.imageElements.transparencyMax);
                }
            }
            else
            {
                foreach (Image image in unit.health.healthBar)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, unit.imageElements.transparencyMin);
                }
            }
        }

        foreach (UnitConfig unit in enemyUnits)
        {
            if (!playerTurn && unit.enemyAi.isMyTurn || selectedUnit != null && selectedUnit.animatorS.target != null && selectedUnit.animatorS.target == unit /*|| unit.enemyAi.isHighlighted   CODE FOR IF THE UNIT IS HIGHLIGHTED     */)
            {
                foreach (Image image in unit.imageElements.elements)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, unit.imageElements.transparencyMax);
                }
            }
            else
            {
                foreach (Image image in unit.health.healthBar)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, unit.imageElements.transparencyMin);
                }
            }
        }
    }
}
