using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {
    GameObject[] spawnNodes;
    public GameObject spawnNode;
    public TurnSystem turnSystem;
    int current = 0;

	void Start () {
        spawnNodes = GameObject.FindGameObjectsWithTag("EnemySpawn");
	}

    /*public GameObject[] randomNodes(int amountNodes) USED TO CREATE RANDOM SPAWNNODES
    {
        GameObject[] nodeArray;
        for(int i = 0; i < amountNodes; i++)
        {
            Instantiate(spawnNode, )
        }
    }*/

    public Vector3 getSpawnNode()
    {
        if (current < spawnNodes.Length - 1)
            current++;
        else
            current = 0;

        return spawnNodes[current].transform.position;
    }
}
