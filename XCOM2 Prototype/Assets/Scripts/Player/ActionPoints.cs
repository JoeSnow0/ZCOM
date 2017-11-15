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
        AdjustActionImages();
    }
    //adds actions on this unit
    public void AddActions(int addition)
    {
        currentActions += addition;
        if (currentActions > unitConfig.unitClassStats.maxUnitActionPoints)
        {
            currentActions = unitConfig.unitClassStats.maxUnitActionPoints;
        }
        AdjustActionImages();
    }
    //removes actions on this unit
    public void SubtractActions(int subtraction)
    {
        TurnSystem.totalActions -= subtraction;
        currentActions -= subtraction;
        AdjustActionImages();
    }
    //removes all actions on this unit
    public void SubtractAllActions()
    {
        TurnSystem.totalActions -= currentActions;
        currentActions = 0;
        AdjustActionImages();
    }
    //Get stats from class and set 
    public void InitializeActions()
    {
        maxActions = unitConfig.unitClassStats.maxUnitActionPoints;
        currentActions = maxActions;
        AdjustActionImages();




    }
    private void AdjustActionImages()
    {
        //Draw new UI APs
        if (actionPointParent != null)
        {
            //Destroy all
            if(actionPointParent.transform.childCount != maxActions)
            {
                foreach (Transform child in actionPointParent.transform)
                {
                    Destroy(child.gameObject);
                }
                //create new ones
                for (int i = 0; i < maxActions; i++)
                {
                    actionPoints.Add(Instantiate(actionPoint, actionPointParent.transform).GetComponent<Image>());
                }
            }
            
        }
        //change their color
        if (actionPoints != null && unitConfig.isFriendly)
        {
           
            //Inactive color
            for (int i = 0; i < maxActions; i++)
            {
                actionPoints[i].color = unitConfig.unitColor[1];
            }
            //Active color
            for (int i = 0; i < currentActions; i++)
            {
                actionPoints[i].color = unitConfig.unitColor[0];
            }
        }
    }
}
