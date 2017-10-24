using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[Serializable]
//Use this to add more preset settings for UI buttons and then assign them in the color editor scriptable object.
//Creates a scriptable object with the parameters below
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
//This adds an update button to the inspector so that you can see the changes
[CustomEditor(typeof(ColorScriptableObject))]
public class UpdateColorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
    ColorScriptableObject myScript = (ColorScriptableObject)target;
        GUILayout.Label("After you press Update, you need to interact with another \n setting to see the changes.");
        if (GUILayout.Button("Update Changes"))
        {
            MassColorChange.updateButtonColor(myScript);

            Repaint();
        }
    }
}