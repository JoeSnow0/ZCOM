using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour {

    public GameObject[] allUnits;
    public GameObject moveToUnit;
    public UnitConfig unitConfig;
    public MapConfig mapConfig;
    
    int damage;
    
	void Update ()
    {
        //HACK: AI Movement?
        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
        if (!mapConfig.turnSystem.playerTurn && unitConfig.actionPoints.actions > 0 && unitConfig.isMoving == false)
        {
            FindClosestPlayerUnit();
            UnitConfig closestUnit = moveToUnit.GetComponent<UnitConfig>();
            mapConfig.tileMap.GeneratePathTo(closestUnit.tileX, closestUnit.tileY, gameObject.GetComponent<UnitConfig>());
            //HACK: zombie attack?
            if (unitConfig.currentPath.Count < 3)
            {
                closestUnit.health.TakeDamage(damage);
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
        Vector3 position = transform.position;

        for (int i = 0; i < mapConfig.turnSystem.playerUnits.Count; i++)
        {

            Vector3 diff = mapConfig.turnSystem.playerUnits[i].transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                distance = curDistance;
                moveToUnit = mapConfig.turnSystem.playerUnits[i].gameObject;
            }
        }
    }
}
