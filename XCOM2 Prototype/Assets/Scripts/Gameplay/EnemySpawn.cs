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

    public void SpawnEnemy(UnitConfig enemyPrefab, int numberOfUnits)
    {
        for (int i = 0; i < numberOfUnits; i++)
        {
            UnitConfig enemy = Instantiate(enemyPrefab, RandomPosition(), Quaternion.identity);
            mapConfig.turnSystem.enemyUnits.Add(enemy);
            enemy.InitializeEnemy();
        }
        mapConfig.turnSystem.StartNextEnemy();
    }

    public Vector3 RandomPosition()
    {
        int x;
        int y;
        x = Random.Range(0, (mapConfig.tileMap.mapSizeX - 1));
        y = Random.Range(0, (mapConfig.tileMap.mapSizeY - 1));
        while (mapConfig.tileMap.tiles[x, y] != 0)
        {
            x = Random.Range(0, (mapConfig.tileMap.mapSizeX - 1));
            y = Random.Range(0, (mapConfig.tileMap.mapSizeY - 1));
        }
        return new Vector3(x * (mapConfig.tileMap.offset), 0, y * (mapConfig.tileMap.offset));
    }
}