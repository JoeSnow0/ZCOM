using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour {

    public int tileX;
    public int tileY;
    public TileMap map;
    public List<Node> currentPath = null;

    int moveSpeed = 6;
    private void Update()
    {
        if (currentPath != null)
        {
            int currNode = 0;
            while (currNode < currentPath.Count - 1)
            {
                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x,currentPath[currNode].y);
                Vector3 end   = map.TileCoordToWorldCoord(currentPath[currNode+1].x, currentPath[currNode+1].y);
                Debug.DrawLine(start, end);
                currNode++;
            }
        }
    }
    public void MoveNextTile()
    {
        int remainingMovement = moveSpeed;
        while(remainingMovement > 0){
            if (currentPath == null)
            {
                return;
            }
            //get cost form current tile to next tile
            remainingMovement -= (int)map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);

            // remove the old current node
            currentPath.RemoveAt(0);

            //set current tile on unit
            
            //move to next tile
            tileX = currentPath[0].x;
            tileY = currentPath[0].y;
            transform.position = map.TileCoordToWorldCoord(tileX, tileY);
            if (currentPath.Count == 1)
            {
                //next tile = the goal
                currentPath = null;
            }
            
        }
    }
}
