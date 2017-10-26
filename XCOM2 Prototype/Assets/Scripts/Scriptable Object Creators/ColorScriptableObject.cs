using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[Serializable]
public class ButtonSettings
{
    public Color normalColor;
    public Color highlightColor;
    public Color pressedColor;
    public Color disabledColor;
    [RangeAttribute(1, 5)]
    public int colorMultiplier;
}

[Serializable]
//Use this to add more preset settings for UI buttons and then assign them in the color editor scriptable object.
//Creates a scriptable object with the parameters below
[CreateAssetMenu(fileName = "ColorEditor", menuName = "Class/CreateColorEditor", order = 2)]
public class ColorScriptableObject : ScriptableObject
{
    //[Header("Button Settings")]
    //public List<ButtonSettings> ButtonPresets;
    [Header("Button Type 1 Settings")]
    public Color    normalColor;
    public Color    highlightColor;
    public Color    pressedColor;
    public Color    disabledColor;
    [RangeAttribute(1, 5)]
    public int      colorMultiplier;

    [Header("Button Outline")]
    public Color    effectColor;
    public int      effectDistanceX, effectDistanceY;

    [Header("Button Image")]
    public Sprite   sourceImage;
    public Color    imageColor;
    public Material imageMaterial;

    [Header("Button Text")]
    public Font     primaryFont;
    public Color    textColor;
}
//This adds an update button to the inspector so that you can update your changes
[CustomEditor(typeof(ColorScriptableObject))]
public class UpdateColorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
    ColorScriptableObject myScript = (ColorScriptableObject)target;
        GUILayout.Label("After you press Update, you need to interact with another \n setting to see the changes.");
        if (GUILayout.Button("Update All"))
        {
            MassColorChange.UpdateButtonColor(myScript);
            Repaint();
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            

        }
        if (GUILayout.Button("Reload Scene"))
        {
            Scene current = EditorSceneManager.GetActiveScene();
            EditorSceneManager.OpenScene(current.path);
        }
    }
}