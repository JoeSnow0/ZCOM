using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour {
    public GameObject selectedUnit;
    public TileType[] tileType;
    int[,] tiles;
    int mapSizeX = 50;//map size
    int mapSizeY = 50;
    public float offset;

    private void Start()
    {
        //spawn grid
        GenerateMapData();
        GenerateMapVisual();
    }
    private void Update()
    {
        
    }
    void GenerateMapVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                TileType tt = tileType[tiles[x, y]];
                GameObject go = Instantiate(tt.tileVisualPrefab, new Vector3(x* offset, 0, y*offset), Quaternion.identity);

                ClickebleTile ct = go.GetComponent<ClickebleTile>();
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;
            }
        }
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

    public void MoveSelectedUnitTo(int x, int y)
    {
        selectedUnit.transform.position = new Vector3(x*offset, 0, y*offset);
    }
}
