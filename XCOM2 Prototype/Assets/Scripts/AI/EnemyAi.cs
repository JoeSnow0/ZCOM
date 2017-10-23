using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : UnitConfig {

    public GameObject[] allUnits;
    public GameObject moveToUnit;
    //TurnSystem turnSystem;
    UnitConfig unitConfig;
    //TileMap tileMap;
    Health Health;
    //ActionPoints actionPoints;
    int damage;
    // Use this for initialization
    void Start ()
    {

        
        //if (!isFriendly)
        //{
        //    for (int i = 0; i <= 1; i++)
        //    {
        //        healthBar[i].color = color[i];
        //    }
        //}
        //unitConfig.unitClassStats.maxUnitHealth = health;
        //unitConfig = GetComponent<UnitConfig>();
        //tileMap = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        //turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();

        //for (int i = 0; i < unitConfig.unitClassStats.maxUnitHealth; i++)
        //{
        //    Instantiate(bar, barParent, false);
        //}
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
        if (!turnSystem.playerTurn && actionPoints.actions > 0 && GetComponent<UnitConfig>().isMoving == false)
        {
            FindClosestPlayerUnit();
            UnitConfig closestUnit = moveToUnit.GetComponent<UnitConfig>();
            tileMap.GeneratePathTo(closestUnit.tileX, closestUnit.tileY, gameObject.GetComponent<UnitConfig>());
            
            if (unitConfig.currentPath.Count < 3)
            {
                closestUnit.GetComponent<Health>().TakeDamage(damage);
                actionPoints.actions = 0;
            }
            else
            {
                gameObject.GetComponent<UnitConfig>().EnemyMoveNextTile();
            }

            
        }
    }

    public void FindClosestPlayerUnit()
    {
        
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        for (int i = 0; i < turnSystem.playerUnits.Count; i++)
        {

            Vector3 diff = turnSystem.playerUnits[i].transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                distance = curDistance;
                moveToUnit = turnSystem.playerUnits[i].gameObject;
            }
        }
    }
}
