using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour {
    [Header("Lists with all units")]
    public GameObject[] allUnits;
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    [Header("Actions")]
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
    [Header("Selected Unit")]
    public Unit selectedUnit;

    public GameObject enemyUnit; //Enemy to spawn, can be changed to an array to randomize

    public EnemySpawn enemySpawnNodes;
    public bool playerTurn = true;
    public bool endTurn = false;
    public CameraControl cameraControl;
    public int maxTurns;
    int thisTurn = 1;
    public int[] spawnEnemyTurns; //Which turns that should spawn enemy units

    //CalculationManager calcManager;

    void Start () {
        allUnits = GameObject.FindGameObjectsWithTag("Unit");

        for (int i = 0; i < allUnits.Length; i++)
        {
            if (allUnits[i].GetComponent<Unit>() != null)
            {
                if (allUnits[i].GetComponent<Unit>().isFriendly)
                {
                    playerUnits.Add(allUnits[i].GetComponent<Unit>());
                }
                else
                {
                    enemyUnits.Add(allUnits[i].GetComponent<Unit>());
                }
            }
        }

        totalActions = playerUnits.Count * 2;

        selectedUnit = playerUnits[0];
        selectedUnit.isSelected = true;
        selectedUnit.GetComponent<BaseUnit>().isSelected = true;

        displayAP(true);
    }

	void Update () {
        selectUnit();
        attackUnit();
        if (playerTurn == false)
        {
            bool endturn = true;
            foreach (Unit enemy in enemyUnits)
            {
                if (enemy.actions > 0 || enemy.baseUnit.isMoving)
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
            foreach (Unit unit in playerUnits)
            {
                if (unit.actions > 0 || unit.baseUnit.isMoving)
                {
                    endturn = false;
                    break;
                }
            }
            if (endturn == true)
            {
                selectedUnit.baseUnit.isSelected = false;
                selectedUnit.GetComponent<Unit>().isSelected = false;
                selectedUnit = null;
                hud.pressEnd(true);
            }
        }

    }
    public void displayAP(bool isPlayerTurn)
    {
        /*if (isPlayerTurn)
        {
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].animAP.SetBool("display", true);
            }
            for(int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].animAP.SetBool("display", false);
            }
        }*/
        /*
        else
        {
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].animAP.SetBool("display", false);
            }
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].animAP.SetBool("display", true);
            }
        }
        */
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
                if (hit.collider.GetComponent<Unit>())
                {
                    if (hit.collider.GetComponent<Unit>().isFriendly)
                    {
                        if (selectedUnit != null)
                        {
                            selectedUnit.GetComponent<Unit>().isSelected = false;
                            selectedUnit.GetComponent<BaseUnit>().isSelected = false;
                        }
                        selectedUnit = hit.collider.GetComponent<Unit>();
                        GetComponent<TileMap>().selectedUnit = selectedUnit.baseUnit;
                        selectedUnit.GetComponent<BaseUnit>().isSelected = true;
                        selectedUnit.GetComponent<Unit>().isSelected = true;
                        MoveCameraToTarget(selectedUnit.transform.position, 0);
                    }
                }
            }
        }
    }

    public void MoveCameraToTarget(Vector3 targetPosition, float time)
    {
        cameraControl.MoveToTarget(targetPosition, time);
    }

    void attackUnit()
    {
        if (Input.GetMouseButtonDown(0) && playerTurn) //Checks if it is the players turn
        {
            if (selectedUnit.actions >= 1) //Checks if the unit has enough action points
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.GetComponent<Unit>()) //Checks if the unit hit an enemy
                    {
                        Unit target = hit.collider.GetComponent<Unit>();
                        if (!target.isFriendly) //Checks if the unit hit is not friendly
                        {
                            //Uses current weapon
                            CalculationManager.HitCheck(selectedUnit.unitWeapon);
                            target.TakeDamage(CalculationManager.damage);

                            //Consumes Actions
                            totalActions -= selectedUnit.actions;
                            selectedUnit.actions = 0;
                            selectNextUnit();
                        }
                    }
                }
            }
        }
    }

    public void resetActions(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            totalActions = playerUnits.Count * 2;
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].actions = 2;
            }
        }
        else
        {
            totalActions = enemyUnits.Count * 2;
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].actions = 2;
            }
        }
        playerTurn = isPlayerTurn;
        
    }
    public void selectNextUnit()
    {
        for(int i = 0; i < playerUnits.Count; i++)
        {
            if(playerUnits[i].actions > 0)
            {
                if (selectedUnit != null)
                {
                    selectedUnit.GetComponent<Unit>().isSelected = false;
                    selectedUnit.GetComponent<BaseUnit>().isSelected = false;
                }
                selectedUnit = playerUnits[i];
                GetComponent<TileMap>().selectedUnit = selectedUnit.baseUnit;
                selectedUnit.GetComponent<BaseUnit>().isSelected = true;
                selectedUnit.GetComponent<Unit>().isSelected = true;
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

    public void destroyUnit(Unit unit)
    {
        if (unit.isFriendly)
            playerUnits.Remove(unit);
        else
            enemyUnits.Remove(unit);

        Destroy(unit.gameObject);
        if(enemyUnits.Count <= 0)
        {
            gameOver.SetActive(true);
            gameOverText.text = "VICTORY";
            gameOverText.color = victoryColor;
        }
        else if(playerUnits.Count <= 0)
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
                Unit unitSpawned = Instantiate(enemyUnit, enemySpawnNodes.GetSpawnNode(), Quaternion.identity).GetComponent<Unit>();
                unitSpawned.turnSystem = this;
                enemyUnits.Add(unitSpawned);
            }
        }
    }
}
