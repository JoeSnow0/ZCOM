using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


//Creates a scriptable object with the parameters below
[CreateAssetMenu(fileName = "StatsClassX", menuName = "Class/Create new class", order = 4)]
public class ClassStatsObject : ScriptableObject
{


    [Header("Class Name/Icon")]
    [Tooltip("Name of the class")]
    public string unitClassName;
    public Sprite classIcon;
    [Header("Class Model")]
    public GameObject classModel;

    [Header("Health")]
    [Tooltip("The amount of health this class has")]
    [Range(1, 40)]
    public int maxUnitHealth;
    [Header("Unit Defense")]
    [Tooltip("The amount which enemy aim is reduced when they shoot at this unit")]
    [Range(1, 10)]
    public int unitDefense;
    [Header("Action Points")]
    [Tooltip("Action points are spent when using abilities")]
    [Range(1, 5)]
    public int maxUnitActionPoints;
    [Header("Movement Points")]
    [Tooltip("The amount of tiles a unit can move in a single action")]
    [Range(1, 10)]
    public int maxUnitMovePoints;
    [Header("Movement Cost")]
    [Tooltip("How much many actions movement costs to use")]
    //Default to 1
    public int moveCost = 1;
}