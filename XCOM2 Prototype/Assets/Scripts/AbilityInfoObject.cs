using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AbilityInfo
{
    public string name;
    public KeyCode keybind;
    public Sprite icon;
    public UnityEvent callbackFunction;
}

[CreateAssetMenu(fileName = "AbilityX", menuName = "Class/CreateAbilitySet", order = 1)]
public class AbilityInfoObject : ScriptableObject
{
    public List<AbilityInfo> abilities = new List<AbilityInfo>();
}


 


