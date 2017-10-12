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
    public bool isSprinting = false;

    int pathIndex = 0;
    public float pathProgress;

    public bool isSelected;
    

    Unit unit;
    TurnSystem turnSystem;

    LineRenderer line;

    private void Start()
    {
        
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        Vector3 tileCoords = map.WorldCoordToTileCoord((int)transform.position.x, (int)transform.position.z);//get unit tile coord
        tileX = (int)tileCoords.x;// set unit position on grid
        tileY = (int)tileCoords.z;

        line = GetComponent<LineRenderer>();
        unit = GetComponent<Unit>();

        turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();
    }
    private void Update()
    {
        if (!isSelected && unit.isFriendly)
        {
            currentPath = null;
            line.positionCount = 0;
        }

        if (isMoving == true)
        {
            turnSystem.MoveCameraToTarget(transform.position, 0);
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

                if (turnSystem.playerTurn)
                    line.positionCount = 0;
            }
            
            else//when unit reach location reset spectial stats
            {
                isMoving = false;
                isSprinting = false;
                currentPath = null;
                pathIndex = 0;
                if(turnSystem.playerTurn)
                    turnSystem.MoveCameraToTarget(turnSystem.selectedUnit.transform.position, 0);

                if (unit.actions <= 0)
                {
                    turnSystem.selectNextUnit();
                }
            }
        }
        //draw line need to be fixed cant be seen in the built version
        if (currentPath != null && unit.isFriendly && !isMoving)
        {
            
            if (currentPath.Count < 4)
            {
                turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(turnSystem.lineColors[0], 0.0f), new GradientColorKey(turnSystem.lineColors[0], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1, 1.0f) }
                    );
                line.colorGradient = turnSystem.gradient;
            }
            else if(currentPath.Count < moveSpeed + 2 && unit.actions > 1)
            {
                turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(turnSystem.lineColors[0], 0.0f), new GradientColorKey(turnSystem.lineColors[0], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1f, 0.05f), new GradientAlphaKey(1, 0.95f), new GradientAlphaKey(0, 1.0f) }
                    );
                line.colorGradient = turnSystem.gradient;
            }
            else
            {
                turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(turnSystem.lineColors[1], 0.0f), new GradientColorKey(turnSystem.lineColors[1], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1f, 0.05f), new GradientAlphaKey(1, 0.95f), new GradientAlphaKey(0, 1.0f) }
                    );
                line.colorGradient = turnSystem.gradient;
            }

            int currNode = 0;
            while (currNode < currentPath.Count - 1 && currNode < moveSpeed * unit.actions)
            {
                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);
                line.positionCount = currNode + 1;
                if(currentPath.Count == 2)
                {
                    line.positionCount = 2;
                    line.SetPosition(0, new Vector3(transform.position.x, 0.1f, transform.position.z));
                    line.SetPosition(1, new Vector3(end.x, 0.1f, end.z));
                }
                if (currNode > 0)
                {
                    line.SetPosition(currNode, new Vector3(end.x, 0.1f, end.z));
                }
                else
                    line.SetPosition(currNode, new Vector3(start.x, 0.1f, start.z));
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
            int remainingMovement = moveSpeed * 2;
            int moveTo = currentPath.Count - 1;
            for (int cost = 1; cost < moveTo;cost++)//is the path posseble
            {
                remainingMovement -= (int)map.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1+cost].x, currentPath[1+cost].y);

            }
            if (remainingMovement > moveSpeed )//can you move the unit 
            {
                
                isMoving = true;//start moving in the update
                animaitionSpeed = 2;
                unit.actions--;
                turnSystem.totalActions--;
                return;
            }
            if (remainingMovement > 0 && unit.actions > 1)//can you move the unit 
            {
                isSprinting = true;
                isMoving = true;//start moving in the update
                animaitionSpeed = 4;
                unit.actions = 0;
                turnSystem.totalActions--;
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
                    isMoving = true;
                    unit.actions--;
                }
                return;
            }
        }
    }
}