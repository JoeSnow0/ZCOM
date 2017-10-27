using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour {

    public GameObject[] allUnits;
    public UnitConfig moveToUnit;
    public UnitConfig unitConfig;
    public MapConfig mapConfig;
    int damage;
    
    int posLeftOrRight;
    int posUPOrDown;
    private void Start()
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        unitConfig = gameObject.GetComponent<UnitConfig>();
    }
    void Update ()
    {
        //HACK: AI Movement?
        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
        if (!mapConfig.turnSystem.playerTurn && unitConfig.actionPoints.actions > 0 && unitConfig.isMoving == false)
        {
            FindClosestPlayerUnit();
            mapConfig.tileMap.GeneratePathTo(moveToUnit.tileX + posLeftOrRight, moveToUnit.tileY + posUPOrDown, unitConfig);
            //HACK: zombie attack?
            if (unitConfig.currentPath.Count < 3)
            {
                moveToUnit.health.TakeDamage(damage);
                unitConfig.actionPoints.actions = 0;
            }
            //HACK: move?
            else
            {
                unitConfig.EnemyMoveNextTile();
            }
        }
    }

    public void FindClosestPlayerUnit()
    {
        float distance = Mathf.Infinity;
        foreach (var unit in mapConfig.turnSystem.playerUnits)
        {
            
            mapConfig.tileMap.GeneratePathTo(unit.tileX + 1, unit.tileY, unit);
            if (distance < unit.currentPath.Count && unit.currentPath != null)
            {
                //right
                posLeftOrRight = -1;
                posUPOrDown = 0;
                distance = unit.currentPath.Count;
                moveToUnit = unit;
            }

            mapConfig.tileMap.GeneratePathTo(unit.tileX - 1, unit.tileY, unit);
            if (distance < unit.currentPath.Count && unit.currentPath != null)
            {
                //left
                posLeftOrRight = -1;
                posUPOrDown = 0;
                distance = unit.currentPath.Count;
                moveToUnit = unit;
            }

            mapConfig.tileMap.GeneratePathTo(unit.tileX, unit.tileY + 1, unit);
            if (distance < unit.currentPath.Count && unit.currentPath != null)
            {
                //up
                posLeftOrRight = 0;
                posUPOrDown = 1;
                distance = unit.currentPath.Count;
                moveToUnit = unit;
            }

            mapConfig.tileMap.GeneratePathTo(unit.tileX, unit.tileY - 1, unit);
            if (distance < unit.currentPath.Count && unit.currentPath != null)
            {
                posLeftOrRight = 0;
                posUPOrDown = -1;
                distance = unit.currentPath.Count;
                moveToUnit = unit;
            }
            
        }
    }
}
