
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class UnitConfig : MonoBehaviour
{
    public string unitName;
    public Color[] unitColor;
    //public Transform dmgStartPos;
    //public GameObject floatingDmg;

    //Data from scriptable objects
    public WeaponInfoObject unitWeapon;
    public ClassStatsObject unitClassStats;
    public AbilityInfoObject unitAbilities;

    //Script references, internal
    [HideInInspector]public ActionPoints actionPoints;
    [HideInInspector]public Health health;
    [HideInInspector]public UnitMovement movement;
    public GenerateAbilityButtons generateAbilityButtons;
    public Animator animatorHealthbar;

    //Script References, external
    [HideInInspector]public MapConfig mapConfig;

    //Unit//
    [HideInInspector] public bool isSelected = false;
    public bool isFriendly;
    public GameObject modelController;
    //Unit Position
    public int tileX;
    public int tileY;

    //grid Reference
    public List<Node> currentPath = null;
    public List<Node> currentBulletPath = null;
    List<Node> testDebug = null; // delete all things that has this list later

    public int movePoints;
    [SerializeField]float animaitionSpeed = 0.05f;
    public enum UnitState {Idle, Shooting, Walking, Sprinting, Dead};
    private UnitState currentUnitState;



    public bool isIdle = true;
    public bool isMoving = false;
    public bool isSprinting = false;
    public bool isShooting = false;
    public bool isDead = false;
    public bool isHighlighted = false;

    public AnimationScript animator;

    int pathIndex = 0;
    public float pathProgress;
    LineRenderer line;
    public EnemyAi enemyAi;
    Color currentColor;
    //[HideInInspector]public ImageElements imageElements;
    Vector3 cameraStartPosition;

    public int accuracy;
    //BaseUnitCopy
    void Start()
    {
        //Load models
        //GameObject classModel = Instantiate(unitClassStats.classModel, modelController.transform);

        //GameObject weaponModel = Instantiate(unitWeapon.weaponModel, classModel.GetComponent<WeaponPosition>().hand);
        
        //Initiate Variables//
        //////////////////////
        //Get Unit movement points
        movePoints = unitClassStats.maxUnitMovePoints;
        //get unit tile coordinates

        //Add the map incase its missing
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();


        if (enemyAi == null)
            InitializeEnemy();

        Vector3 tileCoords = mapConfig.tileMap.WorldCoordToTileCoord((int)transform.position.x, (int)transform.position.z);

        //Set unit position on grid
        tileX = (int)tileCoords.x;
        tileY = (int)tileCoords.z;
        
        line = GetComponent<LineRenderer>();

        animator = GetComponentInChildren<AnimationScript>();

        //Make sure scriptable objects are assigned, if not, assign defaults and send message
        /*if (unitWeapon == null)
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
        }*/
        actionPoints = GetComponent<ActionPoints>();
        health = GetComponent<Health>();
        movement = GetComponent<UnitMovement>();
    }

    void Update()
    {
        if(testDebug != null)
        {
            int currNode = 0;
            while(currNode < testDebug.Count-1)
            {
                Vector3 start = mapConfig.tileMap.TileCoordToWorldCoord(testDebug[currNode].x, testDebug[currNode].y) +
                    new Vector3(0, 1, 0);
                Vector3 end = mapConfig.tileMap.TileCoordToWorldCoord(testDebug[currNode+1].x, testDebug[currNode+1].y) +
                   new Vector3(0, 1, 0);

                Debug.DrawLine(start, end, Color.red);
                currNode++;
            }
            
        }
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
            mapConfig.turnSystem.cameraControl.MoveToTarget(transform.position, cameraStartPosition, true);
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
             
                isSprinting = false;
                currentPath = null;
                pathIndex = 0;
                mapConfig.turnSystem.MoveMarker(mapConfig.turnSystem.unitMarker, transform.position);
                if (mapConfig.turnSystem.playerTurn)
                    mapConfig.turnSystem.cameraControl.MoveToTarget(TurnSystem.selectedUnit.transform.position);

                if (actionPoints.CheckAvailableActions(1))
                {
                    mapConfig.turnSystem.KeyboardSelect(true, mapConfig.turnSystem.playerUnits, TurnSystem.selectedUnit);
                }
                else if(actionPoints.CheckAvailableActions(1) && isFriendly && mapConfig.turnSystem.playerTurn)
                {
                    //Current actions can't be accessed anymore, deal with it
                    mapConfig.tileMap.ChangeGridColor(movePoints, actionPoints.ReturnAvailableActions(), this);
                }
            }
        }
        //draw line need to be fixed cant be seen in the built version
        if (currentPath != null && isFriendly && !isMoving)//1 long path
        {

            if (currentPath.Count < movePoints + 2 && actionPoints.CheckAvailableActions(1))//Walk
            {
                currentColor = mapConfig.turnSystem.lineColors[0];
            }
            else//dash
            {
                currentColor = mapConfig.turnSystem.lineColors[1];
            }

            for (int i = 0; i < mapConfig.turnSystem.markerImage.Length; i++)
            {
                mapConfig.turnSystem.markerImage[i].color = currentColor;
            }
            line.startColor = currentColor;
            line.endColor = currentColor;

            int currNode = 0;
            while (currNode < currentPath.Count - 1 && currNode < movePoints * actionPoints.ReturnAvailableActions())
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
                {
                    line.SetPosition(currNode, new Vector3(start.x, 0.1f, start.z));
                }

                currNode++;

                if (line.positionCount > 0 && mapConfig.turnSystem.cursorMarker.position != end)
                {
                    mapConfig.turnSystem.MoveMarker(mapConfig.turnSystem.cursorMarker, end);
                }
            }
        }
    }
    public void InitializeEnemy()
    {
        animatorHealthbar = GetComponentInChildren<Animator>();
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        Vector3 tileCoords = mapConfig.tileMap.WorldCoordToTileCoord((int)transform.position.x, (int)transform.position.z);
        enemyAi = GetComponent<EnemyAi>();
        tileX = (int)tileCoords.x;
        tileY = (int)tileCoords.z;
        mapConfig.tileMap.UnitMapData(tileX, tileY);
        actionPoints.unitConfig = this;
    }

    public void RangedAttack(UnitConfig self, UnitConfig target)
    {
        if (TurnSystem.selectedUnit == null && !TurnSystem.selectedUnit.actionPoints.CheckAvailableActions(1))
        {
            return;
        }
        //if ability has been confirmed by player
        //Shooting target
        if (!target.isFriendly) //Checks if the unit hit is not friendly
        {
            animator.SetAnimationState(0);
            //Calculate the distance between the units
            mapConfig.turnSystem.distance = Vector3.Distance(TurnSystem.selectedUnit.transform.position, TurnSystem.selectedTarget.transform.position);
            //Check if you hit
            CalculationManager.HitCheck(TurnSystem.selectedUnit.unitWeapon, mapConfig.turnSystem.distance);
            //Shoot target
            //Trigger shooting animation
            SetUnitState(UnitState.Shooting);
            isShooting = true;
            animator.AttackStart();
            
            
            
            //Calculate the distance between the units
            mapConfig.turnSystem.distance = Vector3.Distance(TurnSystem.selectedUnit.transform.position, TurnSystem.selectedTarget.transform.position);
            mapConfig.turnSystem.distance /= 2;

            //Spend Actions
            TurnSystem.selectedUnit.actionPoints.SubtractAllActions();
            //Stop targeting mode
            SetUnitState(UnitState.Idle);
            mapConfig.turnSystem.DeselectUnit(TurnSystem.selectedTarget);

        }
    }
    public void MeleeAttack(UnitConfig self, UnitConfig target)
    {
        //Melee attack script goes here
        //hit check
        CalculationManager.HitCheck(TurnSystem.selectedUnit.unitWeapon, mapConfig.turnSystem.distance);
        //Calculate damage
        CalculationManager.DamageDealt(unitWeapon.baseDamage, unitWeapon.numberOfDiceDamage, unitWeapon.numberOfSidesDamage, true);
        //Spend Actions
        TurnSystem.selectedUnit.actionPoints.SubtractAllActions();
    }

    //public void ShootTarget(UnitConfig target)
    //{
        
    //}

    public void MoveNextTile()//start to try to move unit
    {
        if (currentPath == null || isShooting)// if there is no path (or unit shoots) leave function
        {
            return;
        }

        int remainingMovement = movePoints * 2;
        int moveTo = currentPath.Count - 1;
        for (int cost = 1; cost < moveTo; cost++)//is the path possible
        {
            remainingMovement -= (int)mapConfig.tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);
        }
        if (remainingMovement > movePoints)//can you move the unit 
        {
            mapConfig.turnSystem.cameraControl.SetCameraTime(0);
            cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
            //HACK: idle check should replace isSprinting & isMoving if possible
            SetUnitState(UnitState.Walking); //start moving in the update
            mapConfig.tileMap.ResetColorGrid();
            mapConfig.tileMap.removeUnitMapData(tileX, tileY);
            animaitionSpeed = 2;
            actionPoints.SubtractActions(1); //1 should be whatever it costs to move the unit
            return;
        }
        if (remainingMovement > 0 && actionPoints.CheckAvailableActions(1))//can you move the unit 
        {
            mapConfig.turnSystem.cameraControl.SetCameraTime(0);
            cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
            //HACK: idle check should replace isSprinting & isMoving if possible
            SetUnitState(UnitState.Sprinting);
            mapConfig.tileMap.ResetColorGrid();
            mapConfig.tileMap.removeUnitMapData(tileX, tileY);
            animaitionSpeed = 4;
            actionPoints.SubtractAllActions();
            return;
        }
        else//is too far away do not move
        {
            return;
        }
        
    }
    //Change the Units current state (used for animations)
    public void SetUnitState(UnitState setTo)
    {
        currentUnitState = setTo;

    }
    //check the Units current state, returns true if correct
    public bool CheckUnitState(UnitState compareTo)
    {
        if (currentUnitState == compareTo)
        {
            return true;
        }
        else
        {
            return false;
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
            mapConfig.turnSystem.cameraControl.SetCameraTime(0);
            cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
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
                mapConfig.turnSystem.cameraControl.SetCameraTime(0);
                cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
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

    public void Attack()//
    {
        isShooting = true;
    }

    public void GetAccuracy(int targetTileX,int targetTileY)
    {
        ClickebleTile closest = GetClosestPlayersquare(targetTileX, targetTileY);
        mapConfig.tileMap.GeneratePathTo(closest.tileX, closest.tileY, this, true);
        testDebug = currentBulletPath;
        int accuracy = unitWeapon.baseAim;
        int distans = currentBulletPath.Count - 1;
        for (int aim = 1; aim < distans; aim++)//is the path possible
        {
            accuracy += AimReductionAmount(aim + 1);
            if (accuracy <= 0)
                break;
        }
        Debug.Log("Hit chanse: " + accuracy);
        if (accuracy < 0)
            accuracy = 0;
        else if (accuracy > 100)
            accuracy = 100;
    }

    private ClickebleTile GetClosestPlayersquare(int targetTileX, int targetTileY)
    {
        ClickebleTile closest = null;
        float distance = Mathf.Infinity;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                //the loop will not work under the following conditions
                if ((x == 0 && y == 0) || (y != 0 && x != 0))
                    continue;
                if ((y + targetTileY)  < 0 ||
                    (y + targetTileY) > mapConfig.tileMap.mapSizeY - 1 ||
                    (x + targetTileX) < 0 ||
                    (x + targetTileX) > mapConfig.tileMap.mapSizeX - 1)
                    continue;

                Vector3 diff = transform.position - mapConfig.tileMap.tileobjects[x + targetTileX, y + targetTileY].transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)//set new closest location
                {
                    closest = mapConfig.tileMap.tileobjects[x + targetTileX, y + targetTileY];
                    distance = curDistance;
                }
                else if (curDistance <= (distance + 45f) && 
                        (mapConfig.tileMap.tiles[x + targetTileX, y + targetTileY] != 0 &&
                         mapConfig.tileMap.tiles[x + targetTileX, y + targetTileY] != 4))//if location is not the closest but has a cover
                {
                    closest = mapConfig.tileMap.tileobjects[x + targetTileX, y + targetTileY];
                    distance = curDistance;
                }
            }
        }
        return closest;
    }

    private int AimReductionAmount(int location)
    {
        int amount = 0;
        if(mapConfig.tileMap.tiles[currentBulletPath[location].x, currentBulletPath[location].y] != 0 &&
           mapConfig.tileMap.tiles[currentBulletPath[location].x, currentBulletPath[location].y] != 4)//0 = normal grid and 4 = unit place on grid
        {//reduse amount by the value on the tile
            amount -= (int)mapConfig.tileMap.AccuracyFallOf(currentBulletPath[location-1].x, currentBulletPath[location-1].y, currentBulletPath[location].x, currentBulletPath[location].y);
        }
        else if (location < 9 && mapConfig.tileMap.tiles[currentBulletPath[location].x, currentBulletPath[location].y] == 0)
            amount -= unitWeapon.rangeModShort;

        else if (location < 16 && mapConfig.tileMap.tiles[currentBulletPath[location].x, currentBulletPath[location].y] == 0)
            amount -= unitWeapon.rangeModMedium;

        else if (location < 30)
            amount -= unitWeapon.rangeModLong;

        else
            amount -= unitWeapon.rangeModFar;

        Debug.Log("fall of rate" + amount);
        return amount;//return the amount to lose on the current tile location
    }
}
