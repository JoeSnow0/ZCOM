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

    public void SpawnEnemy(UnitConfig enemyPrefab)
    {
        Instantiate(enemyPrefab, RandomPosition(), Quaternion.identity);
    }

    public Vector3 RandomPosition()
    {
        int x;
        int y;
        x = Random.Range(0, (mapConfig.tileMap.mapSizeX - 1));
        y = Random.Range(0, (mapConfig.tileMap.mapSizeY - 1));
        while (mapConfig.tileMap.tiles[x/ Mathf.RoundToInt(mapConfig.tileMap.offset), y/ Mathf.RoundToInt(mapConfig.tileMap.offset)] != 0)
        {
            x = Random.Range(0, (mapConfig.tileMap.mapSizeX - 1) * Mathf.RoundToInt(mapConfig.tileMap.offset));
            y = Random.Range(0, (mapConfig.tileMap.mapSizeY - 1) * Mathf.RoundToInt(mapConfig.tileMap.offset));
        }
        return new Vector3(x * (mapConfig.tileMap.offset), 0, y * (mapConfig.tileMap.offset));
    }
}