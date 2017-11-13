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


    public ClickebleTile[,] tileobjects;
    [HideInInspector]
    public int[,] tiles;
    Node[,] graph;
    Node[,] graphAir;

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
        ////spawn grid
        //GenerateMapData();//run map generate
        //GeneratePathfindingGraph();//run pathfinding
        //GenerateMapVisual();//make the map visuals
        //changedColoredGrid = new List<ClickebleTile>();
    }
    private void Start()
    {
    }
    public void Initialize()
    {
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
        //Wall cover
        {
            tiles[2, 7] = 1;
            tiles[4, 18] = 1;
            tiles[4, 19] = 1;
            tiles[4, 20] = 1;
            tiles[4, 25] = 1;
            tiles[4, 26] = 1;
            tiles[1, 7] = 1;
            tiles[0, 4] = 1;
            tiles[0, 5] = 1;
            tiles[0, 6] = 1;
            tiles[7, 27] = 1;
            tiles[8, 27] = 1;
            tiles[11, 27] = 1;
            tiles[4, 16] = 1;
            tiles[4, 17] = 1;
            tiles[6, 27] = 1;
            tiles[12, 27] = 1;
            tiles[3, 5] = 1;
            tiles[3, 19] = 1;
            tiles[3, 20] = 1;
            tiles[5, 27] = 1;
            tiles[22, 28] = 1;
            tiles[23, 28] = 1;
            tiles[24, 28] = 1;
            tiles[17, 28] = 1;
            tiles[9, 27] = 1;
            tiles[10, 27] = 1;
            tiles[26, 18] = 1;
            tiles[26, 19] = 1;
            tiles[26, 20] = 1;
            tiles[26, 21] = 1;
            tiles[26, 22] = 1;
            tiles[26, 23] = 1;
            tiles[26, 24] = 1;
            tiles[26, 25] = 1;
            tiles[18, 28] = 1;
            tiles[19, 28] = 1;
            tiles[20, 28] = 1;
            tiles[21, 28] = 1;
            tiles[23, 23] = 1;
            tiles[23, 24] = 1;
            tiles[22, 23] = 1;
            tiles[22, 24] = 1;
            tiles[23, 22] = 1;
            tiles[23, 25] = 1;
            tiles[21, 23] = 1;
            tiles[21, 24] = 1;
            tiles[24, 23] = 1;
            tiles[24, 24] = 1;
            tiles[22, 22] = 1;
            tiles[22, 25] = 1;
            tiles[15, 19] = 1;
            tiles[15, 20] = 1;
            tiles[16, 20] = 1;
            tiles[16, 21] = 1;
            tiles[15, 21] = 1;
            tiles[15, 22] = 1;
            tiles[14, 20] = 1;
            tiles[14, 21] = 1;
            tiles[20, 15] = 1;
            tiles[20, 16] = 1;
            tiles[20, 17] = 1;
            tiles[21, 15] = 1;
            tiles[21, 16] = 1;
            tiles[21, 17] = 1;
            tiles[23, 16] = 1;
            tiles[23, 17] = 1;
            tiles[22, 15] = 1;
            tiles[22, 16] = 1;
            tiles[22, 17] = 1;
            tiles[19, 16] = 1;
            tiles[13, 4] = 1;
            tiles[24, 10] = 1;
            tiles[25, 4] = 1;
            tiles[25, 10] = 1;
            tiles[26, 4] = 1;
            tiles[26, 10] = 1;
            tiles[10, 4] = 1;
            tiles[15, 4] = 1;
            tiles[27, 8] = 1;
            tiles[27, 9] = 1;
            tiles[27, 5] = 1;
            tiles[27, 6] = 1;
            tiles[27, 7] = 1;
            tiles[19, 3] = 1;
            tiles[11, 4] = 1;
            tiles[20, 3] = 1;
            tiles[14, 4] = 1;
            tiles[12, 4] = 1;
            tiles[10, 10] = 1;
            tiles[10, 11] = 1;
            tiles[10, 12] = 1;
            tiles[9, 11] = 1;
            tiles[9, 12] = 1;
            tiles[9, 13] = 1;
            tiles[9, 14] = 1;
            tiles[9, 15] = 1;
            tiles[9, 16] = 1;
            tiles[10, 13] = 1;
            tiles[10, 14] = 1;
            tiles[10, 15] = 1;
            tiles[10, 16] = 1;
            tiles[10, 17] = 1;
            tiles[8, 11] = 1;
            tiles[8, 12] = 1;
            tiles[11, 11] = 1;
            tiles[11, 12] = 1;
            tiles[11, 13] = 1;
            tiles[11, 14] = 1;
            tiles[11, 15] = 1;
            tiles[11, 16] = 1;
            tiles[9, 10] = 1;
            tiles[7, 10] = 1;
            tiles[7, 11] = 1;
            tiles[8, 10] = 1;
            tiles[26, 8] = 1;
            tiles[26, 9] = 1;
            tiles[19, 4] = 1;
            tiles[25, 8] = 1;
            tiles[25, 9] = 1;
            tiles[21, 9] = 1;
            tiles[21, 10] = 1;
            tiles[21, 11] = 1;
            tiles[20, 10] = 1;
            tiles[20, 11] = 1;
            tiles[22, 10] = 1;
            tiles[22, 11] = 1;
            tiles[20, 4] = 1;
            tiles[20, 9] = 1;
            tiles[19, 10] = 1;
            tiles[22, 9] = 1;
            tiles[23, 10] = 1;
            tiles[7, 9] = 1;
            tiles[8, 9] = 1;
            
        }

        //Full cover
        {
            tiles[27, 10] = 2;
            tiles[1, 8] = 2;
            tiles[25, 12] = 2;
            tiles[16, 28] = 2;
            tiles[4, 24] = 2;
            tiles[4, 27] = 2;
            tiles[16, 4] = 2;
            tiles[23, 11] = 2;
            tiles[14, 27] = 2;
            tiles[1, 3] = 2;
            tiles[24, 4] = 2;
            tiles[2, 8] = 2;
            tiles[21, 3] = 2;
            tiles[21, 4] = 2;
            tiles[13, 27] = 2;
            tiles[4, 15] = 2;
            tiles[26, 13] = 2;
            tiles[26, 17] = 2;
            tiles[3, 4] = 2;
            tiles[3, 6] = 2;
            tiles[19, 9] = 2;
            tiles[19, 11] = 2;
            tiles[25, 28] = 2;
            tiles[18, 3] = 2;
            tiles[18, 4] = 2;
            tiles[23, 9] = 2;
            tiles[4, 21] = 2;
            tiles[3, 18] = 2;
            tiles[3, 21] = 2;
            tiles[2, 3] = 2;
            tiles[26, 26] = 2;
            tiles[27, 4] = 2;
            tiles[19, 15] = 2;
            tiles[19, 17] = 2;
            tiles[6, 11] = 2;
            tiles[16, 19] = 2;
            tiles[16, 22] = 2;
            tiles[6, 10] = 2;
            tiles[14, 19] = 2;
            tiles[14, 22] = 2;
            tiles[19, 15] = 2;
            tiles[19, 17] = 2;
            tiles[6, 11] = 2;
            tiles[16, 19] = 2;
            tiles[16, 22] = 2;
            tiles[6, 10] = 2;
            tiles[14, 19] = 2;
            tiles[14, 22] = 2;
            tiles[11, 10] = 2;
            tiles[11, 17] = 2;
            tiles[9, 17] = 2;
            tiles[23, 15] = 2;
        }

        //half cover
        {
            tiles[8, 13] = 3;
            tiles[16, 3] = 3;
            tiles[10, 3] = 3;
            tiles[15, 8] = 3;
            tiles[15, 12] = 3;
            tiles[20, 5] = 3;
            tiles[8, 23] = 3;
            tiles[24, 16] = 3;
            tiles[20, 14] = 3;
            tiles[22, 14] = 3;
            tiles[22, 18] = 3;
            tiles[24, 22] = 3;
            tiles[24, 25] = 3;
            tiles[4, 4] = 3;
            tiles[11, 24] = 3;
            tiles[13, 3] = 3;
            tiles[22, 8] = 3;
            tiles[22, 12] = 3;
            tiles[14, 12] = 3;
            tiles[19, 5] = 3;
            tiles[10, 21] = 3;
            tiles[21, 22] = 3;
            tiles[21, 25] = 3;
            tiles[23, 18] = 3;
            tiles[15, 7] = 3;
            tiles[16, 12] = 3;
            tiles[9, 4] = 3;
        }
    }
    public void UnitMapData(int tileX,int tileY)//when a unit gets or change tile run funktion
    {
        tiles[tileX, tileY] = 4;//Unit pos is unwalkeble
    }
    public void removeUnitMapData(int tileX, int tileY)
    {
        tiles[tileX, tileY] = 0;//Unit can walk on tile
    }

    //sourceX and Y is only used for diagonal movement
    public float CostToEnterTile(int sourceX , int sourceY, int targetX, int targetY, bool canFly = false)// get the cost for the movement to an loction
    {

        TileType tt = tileType[tiles[targetX, targetY]];
        if (!tt.isWalkeble && !canFly)//make it so unwalkeble tiles can't be walked on
        {
            return Mathf.Infinity;
        }

        float cost = tt.movemontCost;
        if (sourceX != targetX && sourceY != targetY)//for diagonally movement
        {
            // we moveing diagonally
            cost *= Mathf.Sqrt(2f);
        }

        return cost;
    }
    public float AccuracyFallOf(int sourceX, int sourceY,  int targetX, int targetY)
    {
        TileType tt = tileType[tiles[targetX, targetY]];
        float accuracy = tt.aimReduction;
        if (sourceX != targetX && sourceY != targetY)//for diagonally movement
        {
            // we moveing diagonally
            accuracy *= Mathf.Sqrt(2f);
        }
        return accuracy;
    }

    void GeneratePathfindingGraph()//create a path for units to walk on
    {
        //initialize the array
        graph = new Node[mapSizeX, mapSizeY];
        graphAir = new Node[mapSizeX, mapSizeY];
        //initilaze a Node for each spot in the array
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;

                graphAir[x, y] = new Node();
                graphAir[x, y].x = x;
                graphAir[x, y].y = y;
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
                if (x > 0)
                {
                    graphAir[x, y].neighbours.Add(graphAir[x - 1, y]);
                    if (y > 0)
                        graphAir[x, y].neighbours.Add(graphAir[x - 1, y - 1]);//down left
                    if (y < mapSizeY - 1)
                        graphAir[x, y].neighbours.Add(graphAir[x - 1, y + 1]);// up left
                }
                //try Right
                if (x < mapSizeX - 1)
                {
                    graphAir[x, y].neighbours.Add(graphAir[x + 1, y]);
                    if (y > 0)
                        graphAir[x, y].neighbours.Add(graphAir[x + 1, y - 1]);//down right
                    if (y < mapSizeY - 1)
                        graphAir[x, y].neighbours.Add(graphAir[x + 1, y + 1]);//up right
                }

                //try neighbours up and down
                if (y > 0)
                    graphAir[x, y].neighbours.Add(graphAir[x, y - 1]);
                if (y < mapSizeY - 1)
                    graphAir[x, y].neighbours.Add(graphAir[x, y + 1]);
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

    public void GeneratePathTo(int tileX, int tileY, UnitConfig selected, bool isBullet = false)//(move to X pos, move to Y pos, gameobject that will be moved)
    {
        selectedUnit = selected;
        selectedUnit.currentPath = null;
        selectedUnit.currentBulletPath = null;

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
        
        if(isBullet)
        {
             source = graphAir[
                                selectedUnit.tileX,
                                selectedUnit.tileY
                                ];
             target = graphAir[
                                tileX,
                                tileY
                                ];
        }
        
        dist[source] = 0;
        prev[source] = null;

        //initialize everything to have infinity distance, since
        //we do not know how far a unit can move right now.
        if (!isBullet)
        {
            foreach(Node v in graph)
            {
                if(v!= source)
                {
                    dist[v] = Mathf.Infinity;
                    prev[v] = null;
                }
                unvisited.Add(v);
            }
        }
        else
        {
            foreach (Node v in graphAir)
            {
                if (v != source)
                {
                    dist[v] = Mathf.Infinity;
                    prev[v] = null;
                }
                unvisited.Add(v);
            }
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
                
                float alt = dist[u] + CostToEnterTile(u.x,u.y,v.x,v.y,isBullet);
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
        if(!isBullet)
            selectedUnit.currentPath = currentPath;
        if (isBullet)
            selectedUnit.currentBulletPath = currentPath;
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
        if (changedColoredGrid == null)
            changedColoredGrid = new List<ClickebleTile>();
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
