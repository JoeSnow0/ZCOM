using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UnitConfig : MonoBehaviour {
    
    //public Transform dmgStartPos;
    //public GameObject floatingDmg;
    public GameObject healthBar;
    public Transform healthBarParent;

    //Data from scriptable objects
    public WeaponInfoObject unitWeapon;
    public ClassStatsObject unitClassStats;
    public AbilityInfoObject unitAbilities;
    
    //Script references, internal
    public ActionPoints actionPoints;
    public Health health;
    //Script References, external
    public TileMap tileMap;
    public TurnSystem turnSystem;

    //Unit//
    public bool isSelected = false;
    public bool isFriendly;
    //Unit Position
    public int tileX;
    public int tileY;

    //grid Reference
    public GameObject map;
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
    LineRenderer line;

    //BaseUnitCopy
    void Start()
    {
        //Initiate Variables//
        //////////////////////

        //get unit tile coordinates
        Vector3 tileCoords = tileMap.UnitCoordToWorldCoord((int)transform.position.x, (int)transform.position.z);

        //Set unit position on grid
        tileX = (int)tileCoords.x;
        tileY = (int)tileCoords.z;
        line = GetComponent<LineRenderer>();

        //Make sure scriptable objects are assigned, if not, assign defaults and send message
        if (unitWeapon == null)
        {
            unitWeapon = AssetDatabase.LoadAssetAtPath<WeaponInfoObject>("Assets/Scriptable Object/Pistol.asset");
            Debug.LogWarning("Couldn't find weapon, using default weapon");
        }
        if (unitClassStats == null)
        {
            unitClassStats = AssetDatabase.LoadAssetAtPath<ClassStatsObject>("Assets/Scriptable Object/StatsRookie.asset");
            Debug.LogWarning("Couldn't find Class, using default class");
        }
        if (unitWeapon == null)
        {
            unitAbilities = AssetDatabase.LoadAssetAtPath<AbilityInfoObject>("Assets/Scriptable Object/AbilityRookie.asset");
            Debug.LogWarning("Couldn't find abilities, using default abilities");
        }

        //Add the map incase its missing
        turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();
        tileMap = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();


        //for (int i = 0; i < unitClassStats.maxUnitHealth; i++)
        //{
        //    Instantiate(healthBar, healthBarParent, false);
        //}
    }

    void Update()
    {
        //BaseUnitCopy
        if (!isSelected && isFriendly)
        {
            currentPath = null;
            line.positionCount = 0;
        }

        if (isMoving == true)
        {
            turnSystem.MoveCameraToTarget(transform.position, 0);
            if (currentPath != null && pathIndex < (currentPath.Count - 1))
            {

                Vector3 previousPosition = tileMap.TileCoordToWorldCoord(currentPath[pathIndex].x, currentPath[pathIndex].y);
                Vector3 nextPosition = tileMap.TileCoordToWorldCoord(currentPath[pathIndex + 1].x, currentPath[pathIndex + 1].y);

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

            else//when unit reach location reset special stats
            {
                isMoving = false;
                isSprinting = false;
                currentPath = null;
                pathIndex = 0;
                if (turnSystem.playerTurn)
                    turnSystem.MoveCameraToTarget(turnSystem.selectedUnit.transform.position, 0);

                if (actionPoints.actions <= 0)
                {
                    turnSystem.selectNextUnit();
                }
            }
        }
        //draw line need to be fixed cant be seen in the built version
        if (currentPath != null && isFriendly && !isMoving)
        {

            if (currentPath.Count < 4)
            {
                turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(turnSystem.lineColors[0], 0.0f), new GradientColorKey(turnSystem.lineColors[0], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1, 1.0f) }
                    );
                line.colorGradient = turnSystem.gradient;
            }
            else if (currentPath.Count < moveSpeed + 2 && actionPoints.actions > 1)
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
            while (currNode < currentPath.Count - 1 && currNode < moveSpeed * actionPoints.actions)
            {
                Vector3 start = tileMap.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector3 end = tileMap.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);
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
            for (int cost = 1; cost < moveTo; cost++)//is the path posseble
            {
                remainingMovement -= (int)tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);

            }
            if (remainingMovement > moveSpeed)//can you move the unit 
            {

                isMoving = true;//start moving in the update
                animaitionSpeed = 2;
                actionPoints.actions--;
                turnSystem.totalActions--;
                return;
            }
            if (remainingMovement > 0 && actionPoints.actions > 1)//can you move the unit 
            {
                isSprinting = true;
                isMoving = true;//start moving in the update
                animaitionSpeed = 4;
                actionPoints.actions = 0;
                turnSystem.totalActions--;
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

            int remainingMovement = moveSpeed;
            int moveTo = currentPath.Count - 1;
            for (int cost = 1; cost < moveTo; cost++)//is the path posseble
            {
                remainingMovement -= (int)tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);


            }

            if (remainingMovement > 0)//can you move the unit 
            {
                currentPath.RemoveAt(currentPath.Count - 1);//move unit next to player
                isMoving = true;//start moving in the update
                actionPoints.actions --;
                return;
            }

            else//is too far away do not move
            {

                remainingMovement = moveSpeed;

                for (int i = currentPath.Count - 1; i > remainingMovement; i--)
                {
                    currentPath.RemoveAt(i);
                }
                if (currentPath != null)
                {
                    isMoving = true;
                    actionPoints.actions--;
                }
                return;
            }
        }
    }
}
