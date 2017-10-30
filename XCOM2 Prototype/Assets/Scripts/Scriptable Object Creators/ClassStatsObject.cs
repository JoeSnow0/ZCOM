using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

//Creates a scriptable object with the parameters below
[CreateAssetMenu(fileName = "StatsClassX", menuName = "Class/Create new class", order = 4)]
public class ClassStatsObject : ScriptableObject
{
    
    [Header("Class Name")]
    [Tooltip("Name of the class")]
    public string unitClassName;
    [Header("Health")]
    [Tooltip("The amount of health this class has")]
    [RangeAttribute(1, 40)]
    public int maxUnitHealth;
    [Header("Unit Defense")]
    [Tooltip("The amount which enemy aim is reduced when they shoot at this unit")]
    [RangeAttribute(1, 10)]
    public int unitDefense;
    [Header("Action Points")]
    [Tooltip("Action points are spent when using abilities")]
    [RangeAttribute(1, 5)]
    public int maxUnitActionPoints;
    [Header("Movement Points")]
    [Tooltip("The amount of tiles a unit can move in a single action")]
    [RangeAttribute(1, 10)]
    public int maxUnitMovePoints;
    [Header("Movement Cost")]
    [Tooltip("How much many actions movement costs to use")]
    public int moveCost;
}