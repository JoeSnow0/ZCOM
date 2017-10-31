using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPoints : MonoBehaviour {
    public Animator animAP;
    [HideInInspector]public int actions;
    private int maxActions;
    //public Color[] color;
    public ClassStatsObject unitClassStats;
    public UnitConfig unitConfig;
    List <Image> actionPoints = new List<Image>();
    public GameObject actionPointParent;
    public GameObject actionPoint;

    // Use this for initialization
    void Start ()
    {
        unitConfig = GetComponent<UnitConfig>();
        InitializeActions();
    }
    private void Update()
    {
        if (actionPoints != null)
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
        actions = unitClassStats.maxUnitActionPoints;
    }

    public void AddActions(int addition)
    {
        actions += addition;
        if (actions > unitClassStats.maxUnitActionPoints)
        {
            actions = unitClassStats.maxUnitActionPoints;
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
        actions = unitClassStats.maxUnitActionPoints;

        if (actionPointParent != null && actionPointParent.transform.childCount > 0 && unitConfig.isFriendly)//Removes any gameobjects in action point parent and sends an error message
        {
            for(int i = 0; i < actionPointParent.transform.childCount; i++)
            {
                Destroy(actionPointParent.transform.GetChild(i).gameObject);
                Debug.Log("DEST");
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
