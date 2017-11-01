using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPoints : MonoBehaviour {
    [HideInInspector]public int actions;


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
        if (actionPoints != null && unitConfig.isFriendly)
        {
            for (int i = 0; i < actionPoints.Count; i++)
            {
                if (i > actions - 1 && actionPoints[i].color != unitConfig.unitColor[1])
                {
                    actionPoints[i].color = unitConfig.unitColor[1];
                }
                else if(i < actions && actionPoints[i].color != unitConfig.unitColor[0])
                {
                    actionPoints[i].color = unitConfig.unitColor[0];
                }
            }
        }

        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
    }

    public void ReplenishAllActions()
    {
        actions = unitConfig.unitClassStats.maxUnitActionPoints;
    }

    public void AddActions(int addition)
    {
        actions += addition;
        if (actions > unitConfig.unitClassStats.maxUnitActionPoints)
        {
            actions = unitConfig.unitClassStats.maxUnitActionPoints;
        }
    }

    public void SubtractActions(int subtraction)
    {
        actions -= subtraction;
    }

    public void SubtractAllActions()
    {
        actions = 0;
    }
    private void InitializeActions()
    {
        actions = unitConfig.unitClassStats.maxUnitActionPoints;

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
            for (int i = 0; i < actions; i++)
            {
                actionPoints.Add(Instantiate(actionPoint, actionPointParent.transform).GetComponent<Image>());
            }
        }
    }
}
