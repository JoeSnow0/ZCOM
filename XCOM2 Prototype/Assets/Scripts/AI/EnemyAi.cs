﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : Unit {

    public GameObject[] allUnits;
    public GameObject moveToUnit;
    
    // Use this for initialization
    void Start ()
    {

        allUnits = GameObject.FindGameObjectsWithTag("Unit");
        if (!isFriendly)
        {
            for (int i = 0; i <= 1; i++)
            {
                healthBar[i].color = color[i];
            }
        }
        healthMax = health;
        healthText.text = health + "/" + healthMax;
        baseUnit = GetComponent<BaseUnit>();
        tileMap = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        apText.text = "(" + actions + ")";

        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
        if (!turnSystem.playerTurn && GetComponent<Unit>().actions > 0 && GetComponent<BaseUnit>().isMoving == false)
        {
            FindClosestPlayerUnit();
            BaseUnit closestUnit = moveToUnit.GetComponent<BaseUnit>();
            tileMap.GeneratePathTo(closestUnit.tileX, closestUnit.tileY, gameObject.GetComponent<BaseUnit>());
            Debug.Log("count = " + baseUnit.currentPath.Count);
            if (baseUnit.currentPath.Count < 3)
            {
                closestUnit.GetComponent<Unit>().TakeDamage(damage);
                actions = 0;
            }
            else
            {
                gameObject.GetComponent<BaseUnit>().EnemyMoveNextTile();
            }

            
        }
    }

    public void FindClosestPlayerUnit()
    {
        
        List<Unit> playerUnits = new List<Unit>();

        for (int i = 0; i < allUnits.Length; i++)
        {
            if (allUnits[i].GetComponent<Unit>()!= null)
            {
                if (allUnits[i].GetComponent<Unit>().isFriendly)
                {
                    playerUnits.Add(allUnits[i].GetComponent<Unit>());
                }
            }
        }

        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        for (int i = 0; i < playerUnits.Count; i++)
        {

            Vector3 diff = playerUnits[i].transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                distance = curDistance;
                moveToUnit = playerUnits[i].gameObject;
            }
        }
    }
}
