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



    void Start ()
    {
        generateButtons = FindObjectOfType<generateButtons>();
        enemySpawn = GetComponent<EnemySpawn>();
        allUnits = FindObjectsOfType<UnitConfig>();
        mapConfig = FindObjectOfType<MapConfig>();
        
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
        selectedUnit = playerUnits[0];
        selectedUnit.isSelected = true;
        foreach (var unit in allUnits)
        {
            mapConfig.tileMap.UnitMapData(unit.tileX, unit.tileY);
        }
        mapConfig.tileMap.ChangeGridColor(selectedUnit.movePoints,selectedUnit.actionPoints.actions,selectedUnit);
        int loopnumber = 0;
        foreach (SpawnSetup setup in spawnSetup)
        {
            setup.activatTurn = loopnumber;
            loopnumber++;
        }
    }
	void Update () {

        attackUnit();
        if (Input.GetKeyDown(nextTarget) && playerTurn)
        {

            SwitchFocusTarget(true);
        }
        if (Input.GetKeyDown(previousTarget) && playerTurn)
        {
            SwitchFocusTarget(false);
        }

        //Mouse select
        
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
                    selectUnit();
                    }

                }
            }
        
        //

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
                MoveCameraToTarget(selectedUnit.transform.position, 0);
            }
            cursorAnimator.SetBool("display", false);
            unitMarkerAnimator.SetBool("display", false);
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
    
    public void selectUnit()
    {
        mapConfig.tileMap.selectedUnit = selectedUnit;

        selectedUnit.isSelected = true;
        //Move the marker to selected unit
        MoveMarker(unitMarker, selectedUnit.transform.position);
        //Move the camera to selected Unit
        MoveCameraToTarget(selectedUnit.transform.position, 0);
        //Update grid colors
        mapConfig.tileMap.ChangeGridColor(selectedUnit.movePoints, selectedUnit.actionPoints.actions, selectedUnit);
        //Clear old abilities
        generateButtons.ClearCurrentButtons();
        //Generate new abilities buttons
        generateButtons.GenerateCurrentButtons(selectedUnit.unitAbilities);
                
        
    }
    public void SwitchFocusTarget(bool nextTarget)
    {
        int currentUnitIndex;
        //get turnsystem player list
        //check if list is empty
        if (playerUnits != null)
        {
            if (selectedUnit != null)
            {
                currentUnitIndex = playerUnits.FindIndex(a => a == selectedUnit);
            }
            else
            {
                selectedUnit = playerUnits[0];
                currentUnitIndex = playerUnits.FindIndex(a => a == selectedUnit);
            }

             
            //move to next unit in list if true
            if (nextTarget)
            {
                //loops around to the beginning of the list
                currentUnitIndex += 1;
                if (currentUnitIndex > playerUnits.Count)
                {
                    currentUnitIndex = 0;
                }
            }

            //move to previous unit in list if false
            if (!nextTarget)
            {
                //loops around to the end of the list
                currentUnitIndex -= 1;
                if (currentUnitIndex < 0)
                {
                    currentUnitIndex = playerUnits.Count - 1;
                }
            }
            //Select the next/previous unit
            selectedUnit = playerUnits[currentUnitIndex % playerUnits.Count];
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
                            //Uses current weapon
                            CalculationManager.HitCheck(selectedUnit.unitWeapon);
                            selectedUnit.ShootTarget(target);


                            //Spend Actions
                            //totalActions -= selectedUnit;
                            //selectedUnit.actionPoints.SubtractAllActions();
                            //selectNextUnit();
                        }
                    }
                }
            }
        }
    }

    public void MoveMarker(Transform m_Marker, Vector3 m_Position)
    {
        if (cursorAnimator.GetBool("display") == false)
        {
            cursorAnimator.SetBool("display", true);
            unitMarkerAnimator.SetBool("display", true);
        }
        m_Marker.position = m_Position;
    }

    public void resetActions(bool isPlayerTurn)
    {
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
                //enemyUnits[i].enemyAI.isBusy = false;
                totalActions += enemyUnits[i].actionPoints.actions;
            }
        }
        playerTurn = isPlayerTurn;
    }

    public void selectNextUnit()
    {
        for(int i = 0; i < playerUnits.Count; i++)
        {
            if(playerUnits[i].actionPoints.actions > 0)
            {
                if (selectedUnit != null)
                {
                    selectedUnit.isSelected = false;
                }
                selectedUnit = playerUnits[i];
                selectedUnit.isSelected = true;
                MoveMarker(unitMarker, selectedUnit.transform.position);
                MoveCameraToTarget(selectedUnit.transform.position, 0);
                mapConfig.tileMap.ChangeGridColor(selectedUnit.movePoints, selectedUnit.actionPoints.actions, selectedUnit);
                break;
            }
        }
    }
    public void StartNextEnemy()
    {
        if (enemyUnits == null ||
            enemyUnits[enemyIndex] == null ||
            enemyUnits[enemyIndex].enemyAi == null)
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
        
        foreach (SpawnSetup i in spawnSetup) // Checks if current turn should spawn an enemy
        {
            if(i.activatTurn == thisTurn)
            {
                enemySpawn.SpawnEnemy(i.enemyPrefab,i.spawnNumberOfEnemys);
                break;
            }
            else if (spawnSetup.Length < thisTurn)
            {
                int newI = Random.Range(0, spawnSetup.Length);
                enemySpawn.SpawnEnemy(spawnSetup[newI].enemyPrefab,spawnSetup[newI].spawnNumberOfEnemys);
                break;
            }
        }
    }

    public void MoveCameraToTarget(Vector3 targetPosition, float time)
    {
        cameraControl.MoveToTarget(targetPosition, time);
    }
}
