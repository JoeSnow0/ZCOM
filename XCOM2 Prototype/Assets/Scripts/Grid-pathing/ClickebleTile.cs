using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickebleTile : MonoBehaviour {
    public int tileX;
    public int tileY;
    public TileMap map;
    void OnMouseUp()//send info to the curent unit
    {
        map.GeneratePathTo(tileX, tileY);
    }

}
