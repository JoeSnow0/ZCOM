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
    public UnitConfig selectedUnit;
    public MapConfig mapConfig;
    public generateButtons generateButtons;
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
    

    //Input
    public KeyCode nextTarget;
    public KeyCode previousTarget;

    public bool EnemyTargeting;
    
    //Distance Variable (maybe put elsewhere?)
    public float distance;

    private UnitConfig lastSelectedUnit;

    void Start ()
    {
        mapConfig = FindObjectOfType<MapConfig>();
        mapConfig.tileMap.Initialize();
        generateButtons = FindObjectOfType<generateButtons>();
        enemySpawn = GetComponent<EnemySpawn>();
        allUnits = FindObjectsOfType<UnitConfig>();

        classInformationAnimator = classIcon.transform.GetComponentInParent<Animator>();
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

        //HACK: What if a unit has more than 2 actions?
        totalActions = playerUnits.Count * 2;
        foreach (var unit in allUnits)
        {
            mapConfig.tileMap.UnitMapData(unit.tileX, unit.tileY);
        }
        SelectFirstUnit();
        
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

        if (!playerTurn && selectedUnit != null)
        {
            DeselectAllUnits();
        }

        if (playerTurn)
        {
            if (Input.GetKeyDown(nextTarget))
            {
                SwitchFocusTarget(true);
            }
            if (Input.GetKeyDown(previousTarget))
            {
                SwitchFocusTarget(false);
            }
        }

        if (!playerTurn && selectedUnit != null) //Deselects unit when it's the enemy turn
            {
                selectedUnit.isSelected = false;
                selectedUnit = null;
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
                            selectUnit();
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
        unit.isSelected = false;
    }
    public void DeselectAllUnits()
    {
        selectedUnit = null;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].isSelected = false;
        }
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].isSelected = false;
        }

    }

    public void selectUnit()
    {
        mapConfig.tileMap.selectedUnit = selectedUnit;

        selectedUnit.isSelected = true;
        //Move the marker to selected unit
        MoveMarker(unitMarker, selectedUnit.transform.position);
        //Move the camera to selected Unit
        cameraControl.MoveToTarget(selectedUnit.transform.position);
        //Update grid colors
        if(playerTurn)
            mapConfig.tileMap.ChangeGridColor(selectedUnit.movePoints, selectedUnit.actionPoints.actions, selectedUnit);


        //HACK: Buttons are broken uncomment when fixed
        ////Clear old abilities
        //generateButtons.ClearCurrentButtons();
        //if (selectedUnit.isFriendly == true)
        //{
        //    //Generate new abilities buttons if its a player unit
        //    generateButtons.GenerateCurrentButtons(selectedUnit.unitAbilities);
        //}
                
        
    }
    //public void SwitchAttackTarget(bool nextTarget)
    //{
    //    int currentUnitIndex;

    //    //check if list is empty
    //    if (enemyUnits != null)
    //    {
    //        if (selectedUnit != null)
    //        {
    //            currentUnitIndex = enemyUnits.FindIndex(a => a == selectedUnit);
    //        }
    //        //If its empty, pick the first friendly unit in list
    //        else
    //        {
    //            selectedUnit = enemyUnits[0];
    //            currentUnitIndex = enemyUnits.FindIndex(a => a == selectedUnit);
    //        }

    //        //Check if any units have actions left
    //        bool UnitHasActionsLeft = false;
    //        foreach (UnitConfig unit in enemyUnits)
    //        {
    //            if (unit.actionPoints.actions > 0)
    //            {
    //                UnitHasActionsLeft = true;
    //                break;
    //            }
    //        }
    //        if (UnitHasActionsLeft == false)
    //        {
    //            return;
    //        }

    //        //move to next unit in list if true
    //        if (nextTarget)
    //        {
    //            for (int i = 0; i < enemyUnits.Count; i++)
    //            {
    //                //loops around to the beginning of the list
    //                currentUnitIndex += 1;
    //                if (currentUnitIndex > enemyUnits.Count)
    //                {
    //                    currentUnitIndex = 0;
    //                }

    //                selectedUnit = enemyUnits[currentUnitIndex % enemyUnits.Count];
    //                if (selectedUnit.actionPoints.actions > 0)
    //                {
    //                    break;
    //                }
    //            }
    //        }

    //        else
    //        {
    //            //move to previous unit in list if false
    //            for (int i = 0; i < enemyUnits.Count; i++)
    //            {

    //                //loops around to the end of the list
    //                currentUnitIndex -= 1;
    //                if (currentUnitIndex < 0)
    //                {
    //                    currentUnitIndex = enemyUnits.Count - 1;
    //                }
    //                selectedUnit = enemyUnits[currentUnitIndex % enemyUnits.Count];
    //                if (selectedUnit.actionPoints.actions > 0)
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //        //Select the next/previous unit
    //        selectUnit();
    //    }
    //}

    public void SwitchFocusTarget(bool nextTarget)
    {
        int currentUnitIndex;

        //check if list is empty
        if (playerUnits != null)
        {
            if (selectedUnit != null)
            {
                currentUnitIndex = playerUnits.FindIndex(a => a == selectedUnit);
            }
            //If its empty, pick the first friendly unit in list
            else
            {
                if (selectedUnit != null)
                    selectedUnit.isSelected = false;

                selectedUnit = playerUnits[0];
                selectedUnit.isSelected = true;
                currentUnitIndex = playerUnits.FindIndex(a => a == selectedUnit);
            }

            //Check if any units have actions left
            bool UnitHasActionsLeft = false;
            foreach (UnitConfig unit in playerUnits)
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

            //move to next unit in list if true
            if (nextTarget)
            {
                for (int i = 0; i < playerUnits.Count; i++)
                {
                    //loops around to the beginning of the list
                    currentUnitIndex += 1;
                    if (currentUnitIndex > playerUnits.Count - 1)
                    {
                        currentUnitIndex = 0;
                    }
                    if(selectedUnit != null)
                        selectedUnit.isSelected = false;

                    selectedUnit = playerUnits[currentUnitIndex];
                    selectedUnit.isSelected = true;
                    if (selectedUnit.actionPoints.actions > 0)
                    {
                        
                        break;
                    }
                }
            }

            //move to previous unit in list if false
            else if (!nextTarget)
            {
                for (int i = 0; i < playerUnits.Count; i++)
                {
                    //loops around to the end of the list
                    currentUnitIndex -= 1;
                    if (currentUnitIndex < 0)
                    {
                        currentUnitIndex = playerUnits.Count - 1;
                    }
                    if (selectedUnit != null)
                        selectedUnit.isSelected = false;

                    selectedUnit = playerUnits[currentUnitIndex];
                    selectedUnit.isSelected = true;
                    if (selectedUnit.actionPoints.actions > 0)
                    {

                        break;
                    }
                }
            }
            //Select the next/previous unit
            print(currentUnitIndex);
            selectUnit();
        }
        
    }

    //Moved attack to unitConfig script
    void attackUnit()
    {
        if (Input.GetMouseButtonDown(0) && playerTurn) //Checks if it is the players turn
        {
            if (selectedUnit.actionPoints.actions >= 1) //Checks if the unit has enough action points
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

                            //Calculate the distance between the units
                            distance = Vector3.Distance(selectedUnit.transform.position, target.transform.position);
                            //Uses current weapon
                            CalculationManager.HitCheck(selectedUnit.unitWeapon, distance);
                            selectedUnit.ShootTarget(target);

                            //Calculate the distance between the units
                            distance = Vector3.Distance(selectedUnit.transform.position, target.transform.position);
                            distance /= 2;


                            //Spend Actions
                            totalActions -= selectedUnit.actionPoints.actions;
                            //selectedUnit.actionPoints.SubtractAllActions();
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
        totalActions = 0;
        if (isPlayerTurn)
        {
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].actionPoints.ReplenishAllActions();
                totalActions += playerUnits[i].actionPoints.actions;

            }
        }
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

    public void SelectFirstUnit()
    {
        selectedUnit = playerUnits[0];
        selectedUnit.isSelected = true;
        MoveMarker(unitMarker, selectedUnit.transform.position);
        cameraControl.MoveToTarget(selectedUnit.transform.position);
        if (playerTurn && selectedUnit != null)
            mapConfig.tileMap.ChangeGridColor(selectedUnit.movePoints, selectedUnit.actionPoints.actions, selectedUnit);
    }

    public void SelectNextUnit()
    {
        for(int i = 0; i < playerUnits.Count; i++)
        {
            if(playerUnits[i].actionPoints.actions > 0 && selectedUnit != playerUnits[i])
            {
                if (selectedUnit != null)
                {
                    selectedUnit.isSelected = false;
                }
                selectedUnit = playerUnits[i];
                selectedUnit.isSelected = true;
                MoveMarker(unitMarker, selectedUnit.transform.position);
                cameraControl.MoveToTarget(selectedUnit.transform.position);
                if(playerTurn && selectedUnit != null)
                    mapConfig.tileMap.ChangeGridColor(selectedUnit.movePoints, selectedUnit.actionPoints.actions, selectedUnit);
                break;
            }
        }
        /*if (selectedUnit == null && playerUnits.Count > 0)
        {
            selectedUnit = playerUnits[0];
            selectedUnit.isSelected = true;
        }*/
        
        
    }
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

        foreach (UnitConfig unit in enemyUnits)
        {
            if (!playerTurn && unit.enemyAi.isMyTurn || unit.isHighlighted || selectedUnit != null && selectedUnit.animatorS.target != null && selectedUnit.animatorS.target == unit /*|| unit.enemyAi.isHighlighted   CODE FOR IF THE UNIT IS HIGHLIGHTED     */)
            {
                unit.animatorHealthbar.SetBool("display", true);
            }
            else if(unit.animatorHealthbar != null)
            {
                unit.animatorHealthbar.SetBool("display", false);
            }
        }

        if(selectedUnit != null && playerTurn)
            lastSelectedUnit = selectedUnit;
    }
}
