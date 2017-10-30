using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour {

    public GameObject[] allUnits;
    public UnitConfig moveToUnit;
    public UnitConfig unitConfig;
    public bool isBusy;
    int damage;
    
    int posLeftOrRight;
    int posUPOrDown;

    int distance = int.MaxValue;
    private void Start()
    {
        unitConfig = gameObject.GetComponent<UnitConfig>();
    }
    void Update ()
    {
        //HACK: AI Movement?
        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
        if (!unitConfig.mapConfig.turnSystem.playerTurn && unitConfig.actionPoints.actions > 0 && unitConfig.isMoving == false)
        {
            foreach (UnitConfig unit in mapConfig.turnSystem.playerUnits)
            {
                IsPlayerNextToMe(unit.tileX, unit.tileY);
                
            }
            if (unitConfig.actionPoints.actions < 1)
                return;
            if (!isBusy)
                FindClosestPlayerUnit();

            //HACK: zombie attack?
            if (unitConfig.currentPath == null)
                return;
            
            //HACK: move?
            
            
            unitConfig.EnemyMoveNextTile();
            
        }
    }

    public void FindClosestPlayerUnit()
    {
        isBusy = true;
        distance = int.MaxValue;
        foreach (var unit in mapConfig.turnSystem.playerUnits)
        {            
            FindClosestPlayerLocation(unit);
        }
        mapConfig.tileMap.GeneratePathTo(moveToUnit.tileX + posLeftOrRight, moveToUnit.tileY + posUPOrDown, unitConfig);//make path to the closest position
    }

    private void FindClosestPlayerLocation(UnitConfig unit)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 && y != 0)//if it is looking for a diagonal pos skip to next
                    continue;

                mapConfig.tileMap.GeneratePathTo(unit.tileX + x, unit.tileY + y, unitConfig);
                if (unitConfig.currentPath != null)
                    if (distance > unitConfig.currentPath.Count)
                    {
                        posLeftOrRight = x;
                        posUPOrDown = y;
                        distance = unitConfig.currentPath.Count;
                        moveToUnit = unit;
                    }
            }
        }

        //mapConfig.tileMap.GeneratePathTo(unit.tileX + 1, unit.tileY, unitConfig);
        //if (unitConfig.currentPath != null)
        //    if (distance > unitConfig.currentPath.Count)
        //    {
        //        //right
        //        posLeftOrRight = 1;
        //        posUPOrDown = 0;
        //        distance = unitConfig.currentPath.Count;
        //        moveToUnit = unit;
        //    }

        //mapConfig.tileMap.GeneratePathTo(unit.tileX - 1, unit.tileY, unitConfig);
        //if (unitConfig.currentPath != null)
        //    if (distance > unitConfig.currentPath.Count)
        //    {
        //        //left
        //        posLeftOrRight = -1;
        //        posUPOrDown = 0;
        //        distance = unitConfig.currentPath.Count;
        //        moveToUnit = unit;
        //    }

        //mapConfig.tileMap.GeneratePathTo(unit.tileX, unit.tileY + 1, unitConfig);
        //if (unitConfig.currentPath != null)
        //    if (distance > unitConfig.currentPath.Count)
        //    {
        //        //up
        //        posLeftOrRight = 0;
        //        posUPOrDown = 1;
        //        distance = unitConfig.currentPath.Count;
        //        moveToUnit = unit;
        //    }

        //mapConfig.tileMap.GeneratePathTo(unit.tileX, unit.tileY - 1, unitConfig);
        //if (unitConfig.currentPath != null)
        //    if (distance > unitConfig.currentPath.Count)
        //    {
        //        //down
        //        posLeftOrRight = 0;
        //        posUPOrDown = -1;
        //        distance = unitConfig.currentPath.Count;
        //        moveToUnit = unit;
        //    }
    }
    public void IsPlayerNextToMe(int tileX,int tileY)
    {
        
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 || y == 0)
                    if (tileX == (unitConfig.tileX + x) && tileY == (unitConfig.tileY + y))
                    {
                        moveToUnit.health.TakeDamage(damage);
                        unitConfig.actionPoints.SubtractActions(2);
                        isBusy = true;
                    }
            }
        }
    }
}
