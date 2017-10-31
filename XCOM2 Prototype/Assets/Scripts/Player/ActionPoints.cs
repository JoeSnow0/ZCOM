using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPoints : MonoBehaviour {
    [System.Serializable]
    public class ActionImage
    {
        public Image actionPointFirst;
        public Image actionpointSecond;
    }
    public Animator animAP;
    [HideInInspector]public int actions;
    private int maxActions;
    public ActionImage actionPointsImage;
    public Color[] color;
    public ClassStatsObject unitClassStats;
    public UnitConfig unitConfig;

    // Use this for initialization
    void Start ()
    {
        unitConfig = GetComponent<UnitConfig>();
        InitializeActions();
    }
    private void Update()
    {
        //Draw action points on friendly unit UI
        if(unitConfig.isFriendly)
        {
            if (actions < 2)
            {
                actionPointsImage.actionPointFirst.color = color[0];
                if (actions < 1)
                {
                    actionPointsImage.actionpointSecond.color = color[0];
                }
                else
                {
                    actionPointsImage.actionpointSecond.color = color[1];
                }
            }
            else
            {
                actionPointsImage.actionpointSecond.color = color[1];
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
    } 
}
