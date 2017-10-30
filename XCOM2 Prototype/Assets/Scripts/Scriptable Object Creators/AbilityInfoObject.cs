using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AbilityInfo
{
    [Header("Name of Keybinding")]
    public string name;
    [Header("Keybind")]
    public KeyCode keybind;
    [Header("Icon")]
    public Sprite icon;
    [Header("Description")]
    [Tooltip("This is used to describe what the ability does")]
    public string tooltip;
    [Header("Ability Cost")]
    [Tooltip("How much many actions an ability costs to use")]
    public int abilityCost;
    [Header("Function")]
    [Tooltip("Which function from abilityFunctions that will be used")]
    public UnityEvent callbackFunction;
}
//Creates a scriptable object with the parameters from AbilityInfo
[CreateAssetMenu(fileName = "AbilityX", menuName = "Class/CreateAbilitySet", order = 1)]
public class AbilityInfoObject : ScriptableObject
{
    
    public List<AbilityInfo> abilities = new List<AbilityInfo>();
}


 


