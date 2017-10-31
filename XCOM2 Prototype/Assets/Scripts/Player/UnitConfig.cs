
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UnitConfig : MonoBehaviour
{

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
    public UnitMovement movement;
    public EnemyAi enemyAI;
    public generateButtons generateButtons;
    //Script References, external
    [HideInInspector]public MapConfig mapConfig;

    //Unit//
    [HideInInspector] public bool isSelected = false;
    public bool isFriendly;
    //Unit Position
    public int tileX;
    public int tileY;

    //grid Reference
    public List<Node> currentPath = null;



    public int movePoints;
    [SerializeField]float animaitionSpeed = 0.05f;
    public bool isMoving = false;
    public bool isSprinting = false;
    public bool isShooting = false;
    public bool isDead = false;
    SoldierAnimation animator;

    int pathIndex = 0;
    public float pathProgress;
    LineRenderer line;

    //BaseUnitCopy
    void Start()
    {
        //Initiate Variables//
        //////////////////////
        //Get Unit movement points
        movePoints = unitClassStats.maxUnitMovePoints;
        //get unit tile coordinates

        //Add the map incase its missing
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        Vector3 tileCoords = mapConfig.tileMap.WorldCoordToTileCoord((int)transform.position.x, (int)transform.position.z);

        //Set unit position on grid
        tileX = (int)tileCoords.x;
        tileY = (int)tileCoords.z;
        
        line = GetComponent<LineRenderer>();

        animator = GetComponentInChildren<SoldierAnimation>();

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

    }

    void Update()
    {
        if (!isSelected && isFriendly)
        {
            currentPath = null;
            line.positionCount = 0;
        }
        if (isShooting)
        {
            //Vector3.RotateTowards(rotation, )
        }

        if (isMoving == true)
        {
            mapConfig.turnSystem.MoveCameraToTarget(transform.position, 0);
            if (currentPath != null && pathIndex < (currentPath.Count - 1))
            {

                Vector3 previousPosition = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[pathIndex].x, currentPath[pathIndex].y);
                Vector3 nextPosition = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[pathIndex + 1].x, currentPath[pathIndex + 1].y);

                pathProgress += Time.deltaTime * animaitionSpeed;
                transform.position = Vector3.Lerp(previousPosition, nextPosition, pathProgress);
                //if unit have reached the end of path reset pathprogress and increase pathindex
                if (pathProgress >= 1.0)
                {

                    pathProgress = 0.0f;
                    pathIndex++;
                }
                //set unit tile postition
                tileX = currentPath[pathIndex].x;
                tileY = currentPath[pathIndex].y;

                if (mapConfig.turnSystem.playerTurn && isFriendly)
                    line.positionCount = 0;
            }

            else//when unit reach location reset special stats
            {
                isMoving = false;
                mapConfig.tileMap.UnitMapData(tileX, tileY);
                if (!isFriendly)
                    GetComponent<EnemyAi>().isBusy = false;
                isSprinting = false;
                currentPath = null;
                pathIndex = 0;
                mapConfig.turnSystem.MoveMarker(mapConfig.turnSystem.unitMarker, transform.position);
                if (mapConfig.turnSystem.playerTurn)
                    mapConfig.turnSystem.MoveCameraToTarget(mapConfig.turnSystem.selectedUnit.transform.position, 0);

                if (actionPoints.actions <= 0)
                {
                    mapConfig.turnSystem.selectNextUnit();
                }
                else if(actionPoints.actions > 0 && isFriendly)
                {
                    mapConfig.tileMap.ChangeGridColor(movePoints, actionPoints.actions, this);
                }
            }
        }
        //draw line need to be fixed cant be seen in the built version
        if (currentPath != null && isFriendly && !isMoving)//1 long path
        {

            if (currentPath.Count < 4)
            {
                mapConfig.turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(mapConfig.turnSystem.lineColors[0], 0.0f), new GradientColorKey(mapConfig.turnSystem.lineColors[0], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1, 1.0f) }
                    );
                line.colorGradient = mapConfig.turnSystem.gradient;
                for (int i = 0; i < 2; i++)
                {
                    mapConfig.turnSystem.markerImage[i].color = mapConfig.turnSystem.lineColors[0];
                }
            }
            else if (currentPath.Count < movePoints + 2 && actionPoints.actions > 1)//full length path
            {
                mapConfig.turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(mapConfig.turnSystem.lineColors[0], 0.0f), new GradientColorKey(mapConfig.turnSystem.lineColors[0], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1f, 0.05f), new GradientAlphaKey(1, 0.95f), new GradientAlphaKey(0, 1.0f) }
                    );
                line.colorGradient = mapConfig.turnSystem.gradient;
                for (int i = 0; i < 2; i++)
                {
                    mapConfig.turnSystem.markerImage[i].color = mapConfig.turnSystem.lineColors[0];
                }
            }
            else//dash length path
            {
                mapConfig.turnSystem.gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(mapConfig.turnSystem.lineColors[1], 0.0f), new GradientColorKey(mapConfig.turnSystem.lineColors[1], 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1f, 0.05f), new GradientAlphaKey(1, 0.95f), new GradientAlphaKey(0, 1.0f) }
                    );
                line.colorGradient = mapConfig.turnSystem.gradient;
                for (int i = 0; i < 2; i++)
                {
                    mapConfig.turnSystem.markerImage[i].color = mapConfig.turnSystem.lineColors[1];
                }
            }

            int currNode = 0;
            while (currNode < currentPath.Count - 1 && currNode < movePoints * actionPoints.actions)
            {
                Vector3 start = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector3 end = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);
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

                if (line.positionCount > 0 && mapConfig.turnSystem.cursorMarker.position != end)
                {
                    mapConfig.turnSystem.MoveMarker(mapConfig.turnSystem.cursorMarker, end);
                }
            }
        }
    }
    public void targetEnemyUnit()
    {

    }

    //HACK: Finish this code block when abilities work!
    public void attackUnit(UnitConfig target)
    {
        //Checks if it is the players turn
        if (mapConfig.turnSystem.playerTurn) 
        {
            //Checks if the unit has enough action points
            if (actionPoints.actions > 0) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Checks if the unit clicked on an enemy
                    if (hit.collider.GetComponent<UnitConfig>()) 
                    {
                        target = hit.collider.GetComponent<UnitConfig>();
                        //Checks if the unit hit is not friendly
                        if (!target.isFriendly) 
                        {
                            //Uses current weapon
                            CalculationManager.HitCheck(unitWeapon);
                            target.health.TakeDamage(CalculationManager.damage);

                            //Spend Actions
                            mapConfig.turnSystem.totalActions -= target.actionPoints.actions;
                            actionPoints.SubtractAllActions();
                            //Move camera to next unit
                            mapConfig.turnSystem.selectNextUnit();
                        }
                    }
                }
            }
        }
    }

    public void ShootTarget(UnitConfig target)
    {
        isShooting = true;
        animator.target = target;
    }

    public void MoveNextTile()//start to try to move unit
    {
        if (currentPath == null)// if there is no path leave funktion
        {
            return;
        }

        
        mapConfig.tileMap.removeUnitMapData(tileX, tileY);
        
        int remainingMovement = movePoints * 2;
        int moveTo = currentPath.Count - 1;
        for (int cost = 1; cost < moveTo; cost++)//is the path possible
        {
            remainingMovement -= (int)mapConfig.tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);
        }
        if (remainingMovement > movePoints)//can you move the unit 
        {
            isMoving = true;//start moving in the update
            mapConfig.tileMap.ResetColorGrid();
            animaitionSpeed = 2;
            actionPoints.actions--;
            mapConfig.turnSystem.totalActions--;
            return;
        }
        if (remainingMovement > 0 && actionPoints.actions > 1)//can you move the unit 
        {
            isSprinting = true;
            isMoving = true;//start moving in the update
            mapConfig.tileMap.ResetColorGrid();
            mapConfig.tileMap.removeUnitMapData(tileX, tileY);
            animaitionSpeed = 4;
            actionPoints.actions = 0;
            mapConfig.turnSystem.totalActions--;
            return;
        }
        else//is too far away do not move
        {
            return;
        }
        
    }
    public void EnemyMoveNextTile()//start to try to move unit
    {

        if (currentPath == null)// if there is no path leave funktion
        {
            return;
        }
        mapConfig.tileMap.removeUnitMapData(tileX, tileY);
        int remainingMovement = movePoints;
        int moveTo = currentPath.Count - 1;
        for (int cost = 0; cost < moveTo; cost++)//is the path posseble
        {
            remainingMovement -= (int)mapConfig.tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);
        }

        if (remainingMovement > 0)//can you move the unit 
        {
            isMoving = true;//start moving in the update
            actionPoints.SubtractActions(1);
            return;
        }

        else//is too far away do not move
        {

            remainingMovement = movePoints * 2;

            for (int i = currentPath.Count - 1; i > remainingMovement; i--)
            {
                currentPath.RemoveAt(i);
            }
            if (currentPath != null)
            {
                isMoving = true;
                actionPoints.SubtractActions(2);
            }
            return;
        }
        
    }
    public void Die()//
    {
        isDead = true;
    }

}
