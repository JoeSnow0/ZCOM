using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MapConfig))]

public class EnemySpawn : MonoBehaviour {
    
    public int current;

    [Header("Set max amount of spawn nodes")]
    public int maxNodes;

    MapConfig mapConfig;

	void Start () {
        
        mapConfig = GetComponent<MapConfig>();
        
        current = -1;
    }

    public void SpawnEnemy(TurnSystem.SpawnSetup spawnSetup, int numberOfUnits)
    {
        foreach (var unitType in spawnSetup.enemyPrefab)
        {
            for (int i = 0; i < numberOfUnits; i++)
            {
                if (unitType == null)
                    continue;

                UnitConfig enemy = Instantiate(unitType, RandomPosition(), Quaternion.identity);
                mapConfig.turnSystem.enemyUnits.Add(enemy);
                enemy.InitializeEnemy();
                enemy.actionPoints.ReplenishAllActions();
                //set zombie name
                enemy.unitName = "Zombie Håkan";
                int maxNames = (int)mapConfig.zombieNameGenerator.zombieNames.Count - 1;
                enemy.unitName = "Zombie " + mapConfig.zombieNameGenerator.zombieNames[Random.Range(0, maxNames)].ToString();
            }
        }
        mapConfig.turnSystem.enemyUnits[0].enemyAi.isMyTurn = true;
    }

    private Vector3 RandomPosition()
    {
        if (mapConfig == null)
            mapConfig = GetComponent<MapConfig>();
        int x = -1;
        int y = -1;
        while (x == -1 && y == -1 || mapConfig.tileMap.tiles[x, y] != 0)
        {
            int wall = Random.Range(0, 3);
            switch (wall)
            {
                case 0://left end of map
                    x = 0;
                    y = Random.Range(0, mapConfig.tileMap.mapSizeY - 1);
                    break;
                case 1://top of map
                    x = Random.Range(0, mapConfig.tileMap.mapSizeX - 1);
                    y = mapConfig.tileMap.mapSizeY - 1;
                    break;
                case 2://right end of map
                    x = mapConfig.tileMap.mapSizeX - 1;
                    y = Random.Range(0, mapConfig.tileMap.mapSizeY - 1);
                    break;
                case 3://down end of map
                    x = Random.Range(0, mapConfig.tileMap.mapSizeX - 1);
                    y = 0;
                    break;
            }
        }
        return new Vector3(x * (mapConfig.tileMap.offset), 0, y * (mapConfig.tileMap.offset));
    }
}