using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]

[CreateAssetMenu(fileName = "ColorEditor", menuName = "Class/CreateColorEditor", order = 2)]
public class ColorScriptableObject : ScriptableObject
{
    [Header("Button Settings")]
    public Color normalColor;
    public Color highlightColor;
    public Color pressedColor;
    public Color disabledColor;
    [RangeAttribute(1, 5)]
    public int colorMultiplier;

    [Header("Outline Settings")]
    public Color effectColor;
    public int effectDistanceX, effectDistanceY;

    [Header("Image Settings")]
    public Sprite sourceImage;
    public Color imageColor;
    public Material imageMaterial;

    [Header("Text Settings")]
    public Font primaryFont;
    public Color textColor;



}





