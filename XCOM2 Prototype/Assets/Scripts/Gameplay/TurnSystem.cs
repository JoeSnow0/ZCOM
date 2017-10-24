using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour {
    [Header("Lists with all units")]
    public UnitConfig[] allUnits;
    public List<UnitConfig> playerUnits = new List<UnitConfig>();
    public List<UnitConfig> enemyUnits = new List<UnitConfig>();

    //[Header("Actions")]
    [HideInInspector]
    public int totalActions;
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

    //Enemy to spawn, can be changed to an array to randomize
    public GameObject EnemyUnitSpawnType; 

    //Script refs
    public EnemySpawn enemySpawnNodes;
    public CameraControl cameraControl;
    //public UnitConfig unitConfig;

    public bool playerTurn = true;
    public bool endTurn = false;
    public int maxTurns;
    int thisTurn = 1;
    public int[] spawnEnemyTurns; //Which turns that should spawn enemy units
    

    void Start ()
    {
        allUnits = GameObject.FindObjectsOfType<UnitConfig>();
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
    }

	void Update () {
        selectUnit();
        attackUnit();
        if (!playerTurn)//enemy turn
        {
            bool endturn = true;
            foreach (var enemy in enemyUnits)
            {
                if (enemy.actionPoints.actions > 0 || enemy.GetComponent<UnitConfig>().isMoving)
                {  
                    endturn = false;
                    break;
                }
            }
            if (endturn == true)
            {
                hud.pressEnd(true);
                MoveCameraToTarget(selectedUnit.transform.position, 0);
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
                }
                
                selectedUnit = null;
                hud.pressEnd(true);
            }
        }

    }

    public void selectUnit()
    {
        if (!playerTurn && selectedUnit != null) //Deselects unit when it's the enemy turn
        {
            selectedUnit.isSelected = false;
            selectedUnit = null;
        }

        if (Input.GetMouseButtonDown(0) && playerTurn)
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
                    GetComponent<TileMap>().selectedUnit = selectedUnit;
                    selectedUnit.isSelected = true;
                    MoveMarker(unitMarker, selectedUnit.transform.position);
                    MoveCameraToTarget(selectedUnit.transform.position, 0);
                    TileMap map = GetComponent<TileMap>();
                    map.ChangeGridColor(selectedUnit.baseUnit.moveSpeed, selectedUnit.actions, selectedUnit.baseUnit);
                }
                
            }
        }
    }

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
                            target.health.TakeDamage(CalculationManager.damage);

                            //Spend Actions
                            //totalActions -= selectedUnit;
                            selectedUnit.actionPoints.SubtractAllActions();
                            selectNextUnit();
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
        }
        m_Marker.position = m_Position;
    }

    public void resetActions(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            //HACK: NOT adaptable
            totalActions = playerUnits.Count * 2;
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].GetComponent<ActionPoints>().actions = 2;
            }
        }
        else
        {
            totalActions = enemyUnits.Count * 2;
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].GetComponent<ActionPoints>().actions = 2;
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
                //GetComponent<TileMap>().selectedUnit = selectedUnit.;
                selectedUnit.isSelected = true;
                MoveMarker(unitMarker, selectedUnit.transform.position);
                MoveCameraToTarget(selectedUnit.transform.position, 0);
                break;
            }
        }
    }

    public int getCurrentTurn(int currentTurn)
    {
        if(currentTurn > maxTurns)
            gameOver.SetActive(true);
        thisTurn = currentTurn;
        return maxTurns;
    }

    public void destroyUnit(UnitConfig unit)
    {
        if (unit.isFriendly)
            playerUnits.Remove(unit);
        else
            enemyUnits.Remove(unit);

        //Destroy(unit.gameObject);
        //if(enemyUnits.Count <= 0)
        //{
        //    gameOver.SetActive(true);
        //    gameOverText.text = "VICTORY";
        //    gameOverText.color = victoryColor;
        //}
        if(playerUnits.Count <= 0)
        {
            gameOver.SetActive(true);
            gameOverText.text = "DEFEAT";
            gameOverText.color = defeatColor;
        }
    }
    public void spawnEnemy()
    {
        foreach (int i in spawnEnemyTurns) // Checks if current turn should spawn an enemy
        {
            if(i == thisTurn)
            {
                //GameObject newEnemyUnitSpawned = Instantiate(EnemyUnitSpawnType, enemySpawnNodes.GetSpawnNode(), Quaternion.identity).GetComponent<GameObject>();
                //newEnemyUnitSpawned.turnSystem = this;
                //enemyUnits.Add(gameObject.newEnemyUnitSpawned);
            }
        }
    }

    public void MoveCameraToTarget(Vector3 targetPosition, float time)
    {
        cameraControl.MoveToTarget(targetPosition, time);
    }
}
