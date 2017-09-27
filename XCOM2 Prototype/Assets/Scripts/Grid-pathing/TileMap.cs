using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour {

    public GameObject selectedUnit;

    public TileType[] tileType;

    int[,] tiles;
    Node[,] graph;

    //need fix for more units
    List<Node> currentPath = null;

    int mapSizeX = 50;//map size
    int mapSizeY = 50;
    public float offset;

    private void Start()
    {
        //spawn grid
        GenerateMapData();
        GeneratePathfindingGraph();
        GenerateMapVisual();
    }

    void GenerateMapData()
    {
        //Allocate our map tiles
        tiles = new int[mapSizeX, mapSizeY];

        //creat map tiles
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }

        //spawn a gruppe of unwalkeble terrain
        for (int x = 10; x < 15; x++)
        {
            for (int y = 10; y < 18; y++)
            {
                tiles[x, y] = 1;
            }
        }
        //obstrical
        tiles[4, 4] = 1;
        tiles[6, 4] = 1;
        tiles[5, 4] = 1;
        tiles[5, 5] = 1;
        tiles[5, 6] = 1;
        tiles[5, 7] = 1;
        tiles[6, 7] = 1;

    }
    public class Node
    {
        public List<Node> neighbours;
        public int x;
        public int y;
        public Node()
        {
            neighbours = new List<Node>();
        }
        public float DistanceTo(Node n)
        {
            return Vector2.Distance(
                new Vector2(x, y),
                new Vector2(n.x, n.y));
        }
    }

    void GeneratePathfindingGraph()
    {
        //initialize the array
        graph = new Node[mapSizeX, mapSizeY];
        //initilaze a Node for each spot in the array
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;

            }
        }
        //now that the nodes exist, calculate their neighbours
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                //4-way connected map
                // can change to hex or 8 way
                //can't loop

                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if(x < mapSizeX - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y]);

                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y-1]);
                if (y < mapSizeY - 1)
                    graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
        }
    }

    void GenerateMapVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                TileType tt = tileType[tiles[x, y]];
                GameObject go = Instantiate(tt.tileVisualPrefab, new Vector3(x * offset, 0, y * offset), Quaternion.identity);

                ClickebleTile ct = go.GetComponent<ClickebleTile>();
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;
            }
        }
    }
    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x * offset, 0, y * offset);
    }
    public void GeneratePathTo(int x, int y)
    {
        selectedUnit.GetComponent<BaseUnit>().currentPath = null;

        //Dijkstra function

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source = graph[
                            selectedUnit.GetComponent<BaseUnit>().tileX,
                            selectedUnit.GetComponent<BaseUnit>().tileY
                            ];
        Node target = graph[
                            x,
                            y
                            ];
        dist[source] = 0;
        prev[source] = null;

        //initialize everything to have infinity distance, since
        //we do not know how far a unit can move right now.
        foreach(Node v in graph)
        {
            if(v!= source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }
        while (unvisited.Count > 0)
        {
            //may need inpovments later on!     
            //"u" is going to be the unvisited node with the smallest distance
            Node u = null;
            foreach(Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if(u == target)
            {
                break;//exit while loop
            }

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                float alt = dist[u] +u.DistanceTo(v);
                if (alt < dist[v]) 
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }
        if(prev[target] == null)
        {
            //no route between our target and our source
            return;
        }
        List<Node> currentPath = new List<Node>();
        Node curr = target;
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        currentPath.Reverse();

        selectedUnit.GetComponent<BaseUnit>().currentPath = currentPath;
    }
}
