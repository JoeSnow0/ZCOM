using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor : MonoBehaviour {
    ClickebleTile activeObject;
    ClickebleTile cursorObject;

    TurnSystem turnSystem;
    TileMap map;
    UnitConfig lastHit;

    public GameObject explosionObject;

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
                    if(explosionObject != null)
                    explosionObject.transform.position = activeObject.transform.position;

                    if (turnSystem.playerTurn) {
                        if (turnSystem.selectedPlayer != null && !turnSystem.selectedPlayer.isMoving)
                        {
                            if(map != null && map.currentGrid[cursorObject.tileX, cursorObject.tileY] != 99)
                                map.GeneratePathTo(cursorObject.tileX, cursorObject.tileY, turnSystem.selectedPlayer);
                        }
                    }
                }

                if (Input.GetMouseButtonUp(1) && turnSystem.playerTurn)
                {
                    if (!turnSystem.selectedPlayer.isMoving)
                    {
                        //map.GeneratePathTo(activeObject.tileX, activeObject.tileY, turnSystem.selectedUnit.baseUnit);

                        
                        turnSystem.selectedPlayer.MoveNextTile();
                    }
                }
            }

            if(hitPosition.collider.CompareTag("Unit") || hitPosition.collider.CompareTag("FriendlyUnit"))
            {
                if(lastHit != null && lastHit != hitPosition.collider.GetComponent<UnitConfig>())
                {
                    lastHit.isHighlighted = false;
                }
                lastHit = hitPosition.collider.GetComponent<UnitConfig>();
                lastHit.isHighlighted = true;
            }
            else if(lastHit != null)
            {
                lastHit.isHighlighted = false;
            }
        }
    }
}
