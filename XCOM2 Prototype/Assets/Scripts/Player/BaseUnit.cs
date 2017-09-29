using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour {

    public int tileX;
    public int tileY;
    public TileMap map;
    public List<Node> currentPath = null;

    private Node previousNode;
    private Node nextNode;

    int moveSpeed = 6;
    [SerializeField]
    float animaitionSpeed = 0.05f;
    bool isMoving = false;

    int pathIndex = 0;
    float pathProgress;

    private void Update()
    {
        //Debug.Log(isMoving);
        if (isMoving == true) {
            if (currentPath != null && pathIndex < (currentPath.Count - 1))
            {
                Vector3 previousPosition = map.TileCoordToWorldCoord(currentPath[pathIndex].x, currentPath[pathIndex].y);
                Vector3 nextPosition = map.TileCoordToWorldCoord(currentPath[pathIndex + 1].x, currentPath[pathIndex + 1].y);
                
                pathProgress += Time.deltaTime * animaitionSpeed;
                transform.position = Vector3.Lerp(previousPosition, nextPosition, pathProgress);

                if (pathProgress >= 1.0)
                {
                    pathProgress = 0.0f;
                    pathIndex++;
                }

                tileX = currentPath[pathIndex].x;
                tileY = currentPath[pathIndex].y;
            }
            else
            {
                isMoving = false;
                currentPath = null;
                pathIndex = 0;
            }
        }
        //draw line
        if (currentPath != null)
        {
            int currNode = 0;
            while (currNode < currentPath.Count - 1)
            {
                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);
                Debug.DrawLine(start, end);
                currNode++;
            }
        }
    }
    public void MoveNextTile()
    {
        int remainingMovement = moveSpeed;
        int moveTo = currentPath.Count-1;
        
        
        if (currentPath == null)
        {
            return;
        }
        
        else
        {
            for (int cost = 0; cost < moveTo;cost++)
            {
                remainingMovement -= (int)map.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1+cost].x, currentPath[1+cost].y);
                
               
                if (remainingMovement < 0)
                    break;
            }
            if (remainingMovement >= 0)
            {
                isMoving = true;
            }
            else
            {
                Debug.Log("out of range");
                return;
            }
        }
        ////int remainingMovement = moveSpeed;
        //while(remainingMovement > 0){
        //    if (currentPath == null)
        //    {
        //        return;
        //    }
        //    //get cost form current tile to next tile
        //    remainingMovement -= (int)map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);

        //    // remove the old current node
        //    currentPath.RemoveAt(0);

        //    //set current tile on unit
            
        //    //move to next tile
        //    tileX = currentPath[0].x;
        //    tileY = currentPath[0].y;
        //    transform.position = map.TileCoordToWorldCoord(tileX, tileY);
        //    transform.position = new Vector3(transform.position.x,0f,transform.position.z);
        //    if (currentPath.Count == 1)
        //    {
        //        //next tile = the goal
        //        currentPath = null;
        //    }
            
        //}
    }
}
