using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor : MonoBehaviour {

    public Color defaultColor;
    public Color hoverColor;

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
                    if(activeObject != null)
                        activeObject.GetComponentInChildren<Renderer>().material.color = defaultColor;

                    activeObject = cursorObject;

                    if (turnSystem.playerTurn) {
                        if (!turnSystem.selectedUnit.isMoving)
                            map.GeneratePathTo(cursorObject.tileX, cursorObject.tileY, turnSystem.selectedUnit);
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
                

                
                hit.GetComponentInChildren<Renderer>().material.color = hoverColor;
                
            }
        }
    }
}
