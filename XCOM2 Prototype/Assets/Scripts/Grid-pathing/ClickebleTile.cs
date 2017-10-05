using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickebleTile : MonoBehaviour {
    public int tileX;
    public int tileY;
    public TileMap map;
    public TurnSystem turnSystem;
    void OnMouseUp()//send info to the curent unit
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        map.GeneratePathTo(tileX, tileY,turnSystem.selectedUnit.baseUnit);//path to the klicked tile     
    }

}
