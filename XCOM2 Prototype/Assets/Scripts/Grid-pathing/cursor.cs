using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor : MonoBehaviour {
    ClickebleTile activeObject;
    ClickebleTile cursorObject;

    TurnSystem turnSystem;
    TileMap map;

    private void Start()
    {
        turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
    }
    void Update ()
    {
        GetPointUnderCursor();
    }

    private void GetPointUnderCursor()
    {
        Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitPosition;

        Physics.Raycast(raycast, out hitPosition);
        if (hitPosition.collider)
        {
            if (hitPosition.collider.CompareTag("Ground"))
            {
                GameObject hit = hitPosition.collider.gameObject;
                cursorObject = hit.GetComponent<ClickebleTile>();

                if (activeObject != cursorObject)
                {
                    activeObject = cursorObject;

                    if (turnSystem.playerTurn) {
                        if (!turnSystem.selectedUnit.isMoving)
                        {
                            if(map.currentGrid[cursorObject.tileX, cursorObject.tileY] != 99)
                                map.GeneratePathTo(cursorObject.tileX, cursorObject.tileY, turnSystem.selectedUnit);
                        }
                    }
                }

                if (Input.GetMouseButtonUp(1) && turnSystem.playerTurn)
                {
                    if (!turnSystem.selectedUnit.isMoving)
                    {
                        //map.GeneratePathTo(activeObject.tileX, activeObject.tileY, turnSystem.selectedUnit.baseUnit);

                        
                        turnSystem.selectedUnit.MoveNextTile();
                    }
                }
            }
        }
    }
}
