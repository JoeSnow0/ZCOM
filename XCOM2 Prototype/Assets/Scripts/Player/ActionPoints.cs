using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPoints : MonoBehaviour {
    public Animator animAP;
    public int actions;
    private int maxActions;
    public Image[] actionPointsImage;
    public Color[] color;
    public ClassStatsObject unitClassStats;

    // Use this for initialization
    void Start ()
    {
        InitializeActions();
    }
    private void Update()
    {
        if (actions < 2)
        {
            actionPointsImage[0].color = color[0];
            if (actions < 1)
            {
                actionPointsImage[1].color = color[0];
            }
            else
            {
                actionPointsImage[1].color = color[1];
            }
        }
        else
        {
            actionPointsImage[0].color = color[1];
            actionPointsImage[1].color = color[1];
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
