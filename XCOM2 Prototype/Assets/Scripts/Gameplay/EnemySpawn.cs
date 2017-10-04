using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {
    public List<GameObject> spawnNodes;
    public GameObject spawnNode;
    
    public int current;

    [Header("Set max amount of spawn nodes")]
    public int maxNodes;

    TurnSystem turnSystem;
    TileMap map;

	void Start () {
        turnSystem = GetComponent<TurnSystem>();
        map = GetComponent<TileMap>();

        spawnNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemySpawn"));
        current = -1;
    }

    /*public GameObject[] randomNodes(int amountNodes) USED TO CREATE RANDOM SPAWNNODES
    {
        GameObject[] nodeArray;
        for(int i = 0; i < amountNodes; i++)
        {
            Instantiate(spawnNode, )
        }
    }*/

    public Vector3 GetSpawnNode()
    {
        if (current < spawnNodes.Count - 1)
        {
            current++;
            if (spawnNodes[current].GetComponent<SpawnNode>().isFree)
            {
                return spawnNodes[current].transform.position;
            }
        }
        else
        {
            NewRandomNode();
            current = spawnNodes.Count - 1;
            return spawnNodes[spawnNodes.Count - 1].transform.position;
        }

        return spawnNodes[current].transform.position;
    }

    public void NewRandomNode()
    {
        GameObject newObject = Instantiate(spawnNode, RandomPosition(), Quaternion.identity);
        spawnNodes.Add(newObject);
    }

    public Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(0, 50), 0, Random.Range(0, 50));
    }
}
