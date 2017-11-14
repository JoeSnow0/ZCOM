using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MapConfig))]

public class EnemySpawn : MonoBehaviour {
    public List<GameObject> spawnNodes;
    public GameObject spawnNode;
    
    public int current;

    [Header("Set max amount of spawn nodes")]
    public int maxNodes;

    MapConfig mapConfig;

	void Start () {
        
        mapConfig = GetComponent<MapConfig>();

        spawnNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemySpawn"));
        current = -1;
    }

    public void SpawnEnemy(TurnSystem.SpawnSetup spawnSetup, int numberOfUnits)
    {
        foreach (var unitType in spawnSetup.enemyPrefab)
        {
            for (int i = 0; i < numberOfUnits; i++)
            {
                UnitConfig enemy = Instantiate(unitType, RandomPosition(), Quaternion.identity);
                mapConfig.turnSystem.enemyUnits.Add(enemy);
            }
        }
    }

    private Vector3 RandomPosition()
    {
        if (mapConfig == null)
            mapConfig = GetComponent<MapConfig>();
        int x = 0;
        int y = 0;
        
        while (mapConfig.tileMap.tiles[x, y] != 0)
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