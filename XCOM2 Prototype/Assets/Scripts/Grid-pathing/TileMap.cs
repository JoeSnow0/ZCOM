using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour {

    public UnitConfig selectedUnit;//needs to change for multiple units

    public TileType[] tileType;//walkeble and unwalkeble terain can be fund in here

    [System.Serializable]
    public class GridMaterials
    {
        public Material normalMaterial;
        public Material walkMaterial;
        public Material dashMaterial;
    }


    ClickebleTile[,] tileobjects;
    [HideInInspector]
    public int[,] tiles;
    Node[,] graph;

    public int[,] currentGrid;
    //may need fix for more units
    List<Node> currentPath = null;
    List<ClickebleTile> changedColoredGrid = null;
    List<ClickebleTile> currentneighbour;

    public GridMaterials gridMaterials;
    private UnitConfig playerGridColorChange;
    public int mapSizeX;//map size
    public int mapSizeY;

    public float offset;
    private void Awake()
    {
        tiles = new int[mapSizeX, mapSizeY];
    }
    private void Start()
    {
        //spawn grid
        GenerateMapData();//run map generate
        GeneratePathfindingGraph();//run pathfinding
        GenerateMapVisual();//make the map visuals
        changedColoredGrid = new List<ClickebleTile>();
    }

    void GenerateMapData()//make the grid and it's obsticals.
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
        tiles[9, 8] = 1;
        tiles[10, 8] = 1;
        tiles[0, 9] = 1;
        tiles[1, 5] = 1;
        tiles[8, 1] = 1;
        tiles[7, 2] = 1;
        tiles[6, 7] = 1;
        tiles[1, 7] = 1;
        tiles[8, 9] = 1;
        tiles[9, 10] = 1;
        tiles[3, 8] = 1;
        tiles[8, 6] = 1;
        tiles[9, 4] = 1;
        tiles[9, 12] = 1;
        tiles[1, 1] = 1;
        tiles[9, 0] = 1;
    }
    public void UnitMapData(int tileX,int tileY)//when a unit gets or change tile run funktion
    {
        tiles[tileX, tileY] = 1;//Unit pos is unwalkeble
    }
    public void removeUnitMapData(int tileX, int tileY)
    {
        tiles[tileX, tileY] = 0;//Unit can walk on tile
    }

    public float CostToEnterTile(int sourceX , int sourceY, int targetX, int targetY)// get the cost for the movement to an loction
    {

        TileType tt = tileType[tiles[targetX, targetY]];
        if (tt.isWalkeble == false)//make it so unwalkeble tiles can't be walked on
        {
            return Mathf.Infinity;
        }

        float cost = tt.movemontCost;
        //if (sourceX!= targetX && sourceY != targetY)//for diagonally movement
        //{
        //    // we moveing diagonally
        //    cost *= 2;
        //}
        
        return cost;
    }

    void GeneratePathfindingGraph()//create a path for units to walk on
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
                //4 - way connected map
                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (x < mapSizeX - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y]);

                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                if (y < mapSizeY - 1)
                    graph[x, y].neighbours.Add(graph[x, y + 1]);

                //8 way
                //try Left
                //if (x > 0)
                //{
                //    graph[x, y].neighbours.Add(graph[x - 1, y]);
                //    if (y > 0)
                //        graph[x, y].neighbours.Add(graph[x-1, y-1]);
                //    if (y < mapSizeY - 1)
                //        graph[x, y].neighbours.Add(graph[x-1, y + 1]);
                //}

                ////try Right
                //if (x < mapSizeX - 1)
                //{
                //    graph[x, y].neighbours.Add(graph[x + 1, y]);
                //    if (y > 0)
                //        graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
                //    if (y < mapSizeY - 1)
                //        graph[x, y].neighbours.Add(graph[x + 1, y + 1]);
                //}

                ////try strsit up and down
                //if (y > 0)
                //    graph[x, y].neighbours.Add(graph[x, y - 1]);
                //if (y < mapSizeY - 1)
                //    graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
        }
    }

    void GenerateMapVisual()// make the grid visible
    {
        tileobjects = new ClickebleTile[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                TileType tt = tileType[tiles[x, y]];
                GameObject go = Instantiate(tt.tileVisualPrefab, new Vector3(x * offset, 0, y * offset), Quaternion.identity, transform);
                ClickebleTile ct = go.GetComponent<ClickebleTile>();
                tileobjects[x, y] = ct;
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;
            }
        }
        
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)//world coordinates to tile coordinates
    {
        return transform.position + new Vector3(x * offset, 0, y * offset);
    }

    public Vector3 WorldCoordToTileCoord(int x,int y)//tile to world coordenets
    {
        return transform.position + new Vector3(x / offset, 0, y / offset);
    }
    public bool UnitCanEnterTile(int x , int y)//walkable terrain on the tile?
    {
        return true;
    }

    public void GeneratePathTo(int tileX, int tileY, UnitConfig selected)//(move to X pos, move to Y pos, gameobject that will be moved)
    {
        selectedUnit = selected;
        selectedUnit.currentPath = null;

        if (UnitCanEnterTile(tileX,tileY) == false)
        {
            //clicked on unwalkable terrain
            return;
        }
        if (tileX < 0 || tileX >= mapSizeX)
            return;
        if (tileY < 0 || tileY >= mapSizeY)
            return;
        //Dijkstra function
        //https://sv.wikipedia.org/wiki/Dijkstras_algoritm for more information of the Dijkstra function

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source = graph[
                            selectedUnit.tileX,
                            selectedUnit.tileY
                            ];
        Node target = graph[
                            tileX,
                            tileY
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
                
                float alt = dist[u] + CostToEnterTile(u.x,u.y,v.x,v.y);
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
        //current path is from goal to unit here we reverse it. to make it normal
        currentPath.Reverse();
        
        selectedUnit.currentPath = currentPath;
    }


    public void ChangeGridColor(int movement, int actions, UnitConfig position)
    {
        currentGrid = new int[mapSizeX, mapSizeY];
        playerGridColorChange = position;

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                currentGrid[x, y] = 99;
            }
        }
        ResetColorGrid();
        GetPlayerNeibours(movement, actions);

    }

    public void ResetColorGrid()
    {
        if (changedColoredGrid != null)
        {
            foreach (ClickebleTile grid in changedColoredGrid)//reset all changed colored tiles
            {
                grid.GetComponentInChildren<Renderer>().material = gridMaterials.normalMaterial;
            }
            changedColoredGrid.Clear();//change material back to normal before this point
        }
    }

    public void GetPlayerNeibours(int movement, int actions)
    {
        currentneighbour = new List<ClickebleTile>();
        int currentRun = 0;//how many times has the loop run
        changedColoredGrid.Add(tileobjects[playerGridColorChange.tileX, playerGridColorChange.tileY].GetComponent<ClickebleTile>());//start of color change

        while (currentRun < (movement * actions))
        {
            currentneighbour.Clear();
            foreach (var neighbour in changedColoredGrid)
            {
                if (neighbour == null)//if neighbour does not have a ClickebleTile skip to next neighbour
                    continue;

                GetNeibours(neighbour, currentRun);
            }

            foreach (var neighbour in currentneighbour)//if the neighbour is walkeble or not
            {
                if (!neighbour.clickeble)
                    continue;

                changedColoredGrid.Add(neighbour);//if tile is walkeble add so that we can check it's neighbours next
            }
            currentRun++;
        }
        ChangeColorGrid(movement, actions);
    }

    private void GetNeibours(ClickebleTile neighbourConfig, int currentRun)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (neighbourConfig.tileX + x < 0 || neighbourConfig.tileY + y < 0)
                    continue;
                if (neighbourConfig.tileX + x > mapSizeX - 1 || neighbourConfig.tileY + y > mapSizeY - 1)
                    continue;
                if ((x != 0 && y != 0) || (x == 0 && y == 0))//if it is looking for a diagonal pos skip to next
                    continue;

                if (currentGrid[neighbourConfig.tileX + x, neighbourConfig.tileY + y] > currentRun)//check if has position has been filed
                    if (tiles[neighbourConfig.tileX + x, neighbourConfig.tileY + y] == 0)//is tile walkeble?
                    {
                        currentneighbour.Add(tileobjects[neighbourConfig.tileX + x, neighbourConfig.tileY + y]);
                        currentGrid[neighbourConfig.tileX + x, neighbourConfig.tileY + y] = currentRun;//if the filed position is lower then the former run replace value
                    }
            }
        }
        //if (neighbourConfig.tileX > 0)//get neighbour to the left
        //{
        //    if (currentGrid[neighbourConfig.tileX - 1, neighbourConfig.tileY] > currentRun)//check if has position has been filed
        //        if (tiles[neighbourConfig.tileX - 1, neighbourConfig.tileY] != 1)//is tile walkeble?
        //        {
                
        //            currentneighbour.Add(tileobjects[neighbourConfig.tileX - 1, neighbourConfig.tileY]);
        //            currentGrid[neighbourConfig.tileX - 1, neighbourConfig.tileY] = currentRun;//if the filed position is lower then the former run replace value
        //        }
        //}
        //if (neighbourConfig.tileX < mapSizeX - 1)//get neighbour to the right
        //{
        //    if (currentGrid[neighbourConfig.tileX + 1, neighbourConfig.tileY] > currentRun)//check if has position has been filed
        //        if (tiles[neighbourConfig.tileX + 1, neighbourConfig.tileY] != 1)//is tile walkeble?
        //        {
        //            currentneighbour.Add(tileobjects[neighbourConfig.tileX + 1, neighbourConfig.tileY]);
        //            currentGrid[neighbourConfig.tileX + 1, neighbourConfig.tileY] = currentRun;//if the filed position is lower then the former run replace value
        //        }
        //}
        //if (neighbourConfig.tileY > 0)//get neighbour to the down
        //{
        //    if (currentGrid[neighbourConfig.tileX, neighbourConfig.tileY - 1] > currentRun)//check if has position has been filed
        //        if (tiles[neighbourConfig.tileX, neighbourConfig.tileY - 1] != 1)//is tile walkeble?
        //        {
        //            currentneighbour.Add(tileobjects[neighbourConfig.tileX, neighbourConfig.tileY - 1]);
        //            currentGrid[neighbourConfig.tileX, neighbourConfig.tileY - 1] = currentRun;//if the filed position is lower then the former run replace value
        //        }
        //}
        //if (neighbourConfig.tileY < mapSizeY - 1)//get neighbour to the up
        //{
        //    if (currentGrid[neighbourConfig.tileX, neighbourConfig.tileY + 1] > currentRun)//check if has position has been filed
        //        if (tiles[neighbourConfig.tileX, neighbourConfig.tileY + 1] != 1)//is tile walkeble?
        //        {
        //            currentneighbour.Add(tileobjects[neighbourConfig.tileX, neighbourConfig.tileY + 1]);
        //            currentGrid[neighbourConfig.tileX, neighbourConfig.tileY + 1] = currentRun;//if the filed position is lower then the former run replace value
        //        }
        //}
    }

    public void ChangeColorGrid(int movement, int actions)
    {
        for (int x = (playerGridColorChange.tileX - (movement * actions)); x <= (playerGridColorChange.tileX + (movement * actions)); x++)
        {
            for (int y = (playerGridColorChange.tileY - (movement * actions)); y <= (playerGridColorChange.tileY + (movement * actions)); y++)
            {

                if (y < 0)
                    continue;
                if (y > mapSizeY - 1)
                    continue;
                if (x < 0)
                    continue;
                if (x > mapSizeX - 1)
                    continue;
                if (currentGrid[x, y] == 99)
                    continue;
                if (tiles[x, y] == 1)
                    continue;
                ClickebleTile tile = tileobjects[x, y];
                if (currentGrid[x, y] < movement && actions != 1)
                {
                    tile.GetComponentInChildren<Renderer>().material = gridMaterials.walkMaterial;// walk color

                }
                else if (currentGrid[x, y] >= movement || (currentGrid[x, y] <= movement && actions == 1))//dash if more then movment or if unit only have one action
                {
                    tile.GetComponentInChildren<Renderer>().material = gridMaterials.dashMaterial;//dash color

                }
            }
        }
    }
}
