using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPoints : MonoBehaviour {
    private int currentActions;
    private int maxActions;


    List <Image> actionPoints = new List<Image>();

    public GameObject actionPointParent;
    public GameObject actionPoint;

    [HideInInspector]public UnitConfig unitConfig;
    
    void Start ()
    {
        InitializeActions();
    }
    private void Update()
    {
        //Update UI elements
        if (actionPoints != null && unitConfig.isFriendly)
        {
            for (int i = 0; i < actionPoints.Count; i++)
            {
                if (i > currentActions - 1 && actionPoints[i].color != unitConfig.unitColor[1])
                {
                    actionPoints[i].color = unitConfig.unitColor[1];
                }
                else if(i < currentActions && actionPoints[i].color != unitConfig.unitColor[0])
                {
                    actionPoints[i].color = unitConfig.unitColor[0];
                }
            }
        }

        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
    }
    //returns true if unit has more or equal actions compared to requiredActions
    public bool CheckAvailableActions(int requiredActions)
    {
        if (currentActions >= requiredActions)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //returns available actions
    public int ReturnAvailableActions()
    {
        return currentActions;
    }
    //adds all actions on this unit
    public void ReplenishAllActions()
    {
        currentActions = maxActions;
        TurnSystem.totalActions += currentActions;
    }
    //adds actions on this unit
    public void AddActions(int addition)
    {
        currentActions += addition;
        if (currentActions > unitConfig.unitClassStats.maxUnitActionPoints)
        {
            currentActions = unitConfig.unitClassStats.maxUnitActionPoints;
        }
    }
    //removes actions on this unit
    public void SubtractActions(int subtraction)
    {
        TurnSystem.totalActions -= subtraction;
        currentActions -= subtraction;
    }
    //removes all actions on this unit
    public void SubtractAllActions()
    {
        TurnSystem.totalActions -= currentActions;
        currentActions = 0;
    }
    //Get stats from class and set 
    private void InitializeActions()
    {
        currentActions = unitConfig.unitClassStats.maxUnitActionPoints;
        maxActions = unitConfig.unitClassStats.maxUnitActionPoints;

        if (actionPointParent != null && actionPointParent.transform.childCount > 0 && unitConfig.isFriendly)//Removes any gameobjects in action point parent and sends an error message
        {
            for(int i = 0; i < actionPointParent.transform.childCount; i++)
            {
                Destroy(actionPointParent.transform.GetChild(i).gameObject);
                
            }
            Debug.LogError("Remove action point(s) from action point parent in " + this.name);
        }
        else if (actionPointParent == null && unitConfig.isFriendly)
        {
            Debug.LogError("Action points parent needed in " + this.name);
        }

        if (actionPointParent != null)
        {
            for (int i = 0; i < currentActions; i++)
            {
                actionPoints.Add(Instantiate(actionPoint, actionPointParent.transform).GetComponent<Image>());
            }
        }
    }
}
