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

    public int moveSpeed = 6;
    [SerializeField]
    float animaitionSpeed = 0.05f;
    public bool isMoving = false;

    int pathIndex = 0;
    public float pathProgress;

    public bool isSelected;

    Unit unit;
    TurnSystem turnSystem;

    private void Start()
    {
        
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        Vector3 tileCoords = map.UnitCoordToWorldCoord((int)transform.position.x, (int)transform.position.z);//get unit tile coord
        tileX = (int)tileCoords.x;// set unit position on grid
        tileY = (int)tileCoords.z;

        unit = GetComponent<Unit>();

        turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();
    }
    private void Update()
    {
        
        if (isMoving == true)
        {
            turnSystem.FollowUnit(transform.position, 1);
            if (currentPath != null && pathIndex < (currentPath.Count - 1))
            {
                
                Vector3 previousPosition = map.TileCoordToWorldCoord(currentPath[pathIndex].x, currentPath[pathIndex].y);
                Vector3 nextPosition = map.TileCoordToWorldCoord(currentPath[pathIndex + 1].x, currentPath[pathIndex + 1].y);
                
                pathProgress += Time.deltaTime * animaitionSpeed;
                transform.position = Vector3.Lerp(previousPosition, nextPosition, pathProgress);

                if (pathProgress >= 1.0)//if unit have reached the end of path reset pathprogress and increacss pathindex
                {

                    pathProgress = 0.0f;
                    pathIndex++;
                }

                tileX = currentPath[pathIndex].x;//set unit tile postition
                tileY = currentPath[pathIndex].y;
            }

            else//when unit reach location reset spectial stats
            {
                isMoving = false;
                currentPath = null;
                pathIndex = 0;
            }
        }
        //draw line need to be fixed cant be seen in the built version
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
    public void MoveNextTile()//start to try to move unit
    {
                
        if (currentPath == null)// if there is no path leave funktion
        {
            return;
        }
        
        else
        {
            int remainingMovement = moveSpeed;
            int moveTo = currentPath.Count - 1;
            for (int cost = 1; cost < moveTo;cost++)//is the path posseble
            {
                remainingMovement -= (int)map.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1+cost].x, currentPath[1+cost].y);

            }
            if (remainingMovement > 0)//can you move the unit 
            {
                isMoving = true;//start moving in the update
                unit.actions--;
                turnSystem.totalActions--;
                if (unit.actions <= 0)
                {
                    turnSystem.selectNextUnit();
                }
                return;
            }
            else//is too far away do not move
            {
                Debug.Log("out of range");
                return;
            }
        }
    }
    public void EnemyMoveNextTile()//start to try to move unit
    {

        if (currentPath == null)// if there is no path leave funktion
        {
            Debug.Log("this is a test");
            return;
        }

        else
        {
            
            int remainingMovement = moveSpeed;
            int moveTo = currentPath.Count - 1;
            for (int cost = 1; cost < moveTo; cost++)//is the path posseble
            {
                remainingMovement -= (int)map.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);

                
            }

            if (remainingMovement > 0)//can you move the unit 
            {
                currentPath.RemoveAt(currentPath.Count - 1);//move unit next to player
                isMoving = true;//start moving in the update
                unit.actions--;
                Debug.Log(unit.actions);
                return;
            }

            else//is too far away do not move
            {

                remainingMovement = moveSpeed;

                for (int i = currentPath.Count-1; i > remainingMovement; i--)
                {
                    currentPath.RemoveAt(i);
                }
                if(currentPath != null)
                {
                    Debug.Log(currentPath.Count - 1);
                    isMoving = true;
                    unit.actions--;
                }
                return;
            }
        }
    }
}