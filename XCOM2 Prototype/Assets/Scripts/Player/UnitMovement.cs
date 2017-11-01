using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour {
    //Unit//

    //Unit Position
    public int      tileX;
    public int      tileY;

    //Movement
    private int     movePoints;
    private float   animaitionSpeed = 0.05f;
    public bool     isMoving = false;
    public bool     isSprinting = false;

    //Path
    public List<Node> currentPath = null;
    private int     pathIndex = 0;
    public float    pathProgress;
    LineRenderer    line;
    public ClassStatsObject unitClassStats;

    UnitConfig unitConfig;

    private void Start()
    {
        unitConfig = GetComponent<UnitConfig>();
    }
    private void Update()
    {
        if (!unitConfig.isSelected && unitConfig.isFriendly)
        {
            currentPath = null;
            //uncomment this when script is ready
            //line.positionCount = 0;
        }

        if (isMoving == true)
        {
            unitConfig.mapConfig.turnSystem.MoveCameraToTarget(transform.position, 0);
            if (currentPath != null && pathIndex < (currentPath.Count - 1))
            {

                Vector3 previousPosition = unitConfig.mapConfig.tileMap.TileCoordToWorldCoord(currentPath[pathIndex].x, currentPath[pathIndex].y);
                Vector3 nextPosition = unitConfig.mapConfig.tileMap.TileCoordToWorldCoord(currentPath[pathIndex + 1].x, currentPath[pathIndex + 1].y);

                pathProgress += Time.deltaTime * animaitionSpeed;
                transform.position = Vector3.Lerp(previousPosition, nextPosition, pathProgress);

                //if unit have reached the end of path reset pathprogress and increacss pathindex
                if (pathProgress >= 1.0)
                {

                    pathProgress = 0.0f;
                    pathIndex++;
                }
                //set unit tile postition
                tileX = currentPath[pathIndex].x;
                tileY = currentPath[pathIndex].y;

                if (unitConfig.mapConfig.turnSystem.playerTurn)
                    line.positionCount = 0;
            }

            else//when unit reach location reset special stats
            {
                isMoving = false;
                isSprinting = false;
                currentPath = null;
                pathIndex = 0;
                if (unitConfig.mapConfig.turnSystem.playerTurn)
                    unitConfig.mapConfig.turnSystem.MoveCameraToTarget(unitConfig.mapConfig.turnSystem.selectedUnit.transform.position, 0);

                if (unitConfig.actionPoints.actions <= 0)
                {
                    unitConfig.mapConfig.turnSystem.SelectNextUnit();
                }
            }
        }
        //draw line need to be fixed cant be seen in the built version
        if (currentPath != null && unitConfig.isFriendly && !isMoving)//1 long path
        {

            if (currentPath.Count < 4)
            {
                unitConfig.mapConfig.turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(unitConfig.mapConfig.turnSystem.lineColors[0], 0.0f), new GradientColorKey(unitConfig.mapConfig.turnSystem.lineColors[0], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1, 1.0f) }
                    );
                line.colorGradient = unitConfig.mapConfig.turnSystem.gradient;
                for (int i = 0; i < 2; i++)
                {
                    unitConfig.mapConfig.turnSystem.markerImage[i].color = unitConfig.mapConfig.turnSystem.lineColors[0];
                }
            }
            else if (currentPath.Count < movePoints + 2 && unitConfig.actionPoints.actions > 1)//full length path
            {
                unitConfig.mapConfig.turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(unitConfig.mapConfig.turnSystem.lineColors[0], 0.0f), new GradientColorKey(unitConfig.mapConfig.turnSystem.lineColors[0], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1f, 0.05f), new GradientAlphaKey(1, 0.95f), new GradientAlphaKey(0, 1.0f) }
                    );
                line.colorGradient = unitConfig.mapConfig.turnSystem.gradient;
                for (int i = 0; i < 2; i++)
                {
                    unitConfig.mapConfig.turnSystem.markerImage[i].color = unitConfig.mapConfig.turnSystem.lineColors[0];
                }
            }
            else//dash length path
            {
                unitConfig.mapConfig.turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(unitConfig.mapConfig.turnSystem.lineColors[1], 0.0f), new GradientColorKey(unitConfig.mapConfig.turnSystem.lineColors[1], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1f, 0.05f), new GradientAlphaKey(1, 0.95f), new GradientAlphaKey(0, 1.0f) }
                    );
                line.colorGradient = unitConfig.mapConfig.turnSystem.gradient;
                for (int i = 0; i < 2; i++)
                {
                    unitConfig.mapConfig.turnSystem.markerImage[i].color = unitConfig.mapConfig.turnSystem.lineColors[1];
                }
            }

            int currNode = 0;
            while (currNode < currentPath.Count - 1 && currNode < movePoints * unitConfig.actionPoints.actions)
            {
                Vector3 start = unitConfig.mapConfig.tileMap.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector3 end = unitConfig.mapConfig.tileMap.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);
                line.positionCount = currNode + 1;
                if (currentPath.Count == 2)
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

                if (line.positionCount > 0 && unitConfig.mapConfig.turnSystem.cursorMarker.position != end)
                {
                    unitConfig.mapConfig.turnSystem.MoveMarker(unitConfig.mapConfig.turnSystem.cursorMarker, end);
                }
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
            int remainingMovement = movePoints * unitConfig.actionPoints.actions;
            int moveTo = currentPath.Count - 1;
            for (int cost = 1; cost < moveTo; cost++)//is the path possible
            {
                remainingMovement -= (int)unitConfig.mapConfig.tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);
            }
            if (remainingMovement > movePoints)//can you move the unit 
            {
                isMoving = true;//start moving in the update
                animaitionSpeed = 2;
                //HACK:Subtracts actions, needs to be a variable in stats
                unitConfig.actionPoints.SubtractActions(1);
                unitConfig.mapConfig.turnSystem.totalActions--;
                return;
            }
            if (remainingMovement > 0 && unitConfig.actionPoints.actions > 1)//can you move the unit 
            {
                isSprinting = true;
                isMoving = true;//start moving in the update
                animaitionSpeed = 4;
                unitConfig.actionPoints.SubtractAllActions();
                unitConfig.mapConfig.turnSystem.totalActions--;
                return;
            }
            else//is too far away do not move
            {
                return;
            }
        }
    }
    public void EnemyMoveNextTile()//start to try to move unit
    {

        if (currentPath == null)// if there is no path leave funktion
        {
            //Debug.Log("this is a test");
            return;
        }

        else
        {

            int remainingMovement = movePoints;
            int moveTo = currentPath.Count - 1;
            for (int cost = 1; cost < moveTo; cost++)//is the path posseble
            {
                remainingMovement -= (int)unitConfig.mapConfig.tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);


            }

            if (remainingMovement > 0)//can you move the unit 
            {
                currentPath.RemoveAt(currentPath.Count - 1);//move unit next to player
                isMoving = true;//start moving in the update
                unitConfig.actionPoints.SubtractActions(1);
                return;
            }

            else//is too far away do not move
            {

                remainingMovement = movePoints;

                for (int i = currentPath.Count - 1; i > remainingMovement; i--)
                {
                    currentPath.RemoveAt(i);
                }
                if (currentPath != null)
                {
                    isMoving = true;
                    unitConfig.actionPoints.SubtractActions(1);
                }
                return;
            }
        }
    }
}

