using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

//This code references ColorScriptableObject.cs

[ExecuteInEditMode]
public class MassColorChange : MonoBehaviour {

    static List<MassColorChange> trackedObjects = new List<MassColorChange>();

    static ColorScriptableObject ScriptableColorObject;

    // Use this for initialization
    void Start()
    {
        //Add ColorEditor to ScriptableColorObject
        if (ScriptableColorObject == null)
        {
            ScriptableColorObject = AssetDatabase.LoadAssetAtPath<ColorScriptableObject>("Assets/Scriptable Object/ColorEditor.asset");
        }
        trackedObjects.Add(this);
    }
    
    private void OnDestroy()
    {
        trackedObjects.Remove(this);
    }

    void ApplyColorChange()
    {
        //Update button Components
        Button b = GetComponent<Button>();
        if (b)
        {

            ColorBlock block = b.colors;
            block.normalColor = ScriptableColorObject.normalColor;
            block.highlightedColor = ScriptableColorObject.highlightColor;
            block.pressedColor = ScriptableColorObject.pressedColor;
            block.disabledColor = ScriptableColorObject.disabledColor;
            block.colorMultiplier = ScriptableColorObject.colorMultiplier;
            b.colors = block;

            //return;
        }

        //Update outline Components
        Outline o = GetComponent<Outline>();
        if (o)
        {
            Vector2 shadow = o.effectDistance;
            o.effectColor = ScriptableColorObject.effectColor;
            shadow.x = ScriptableColorObject.effectDistanceX;
            shadow.y = ScriptableColorObject.effectDistanceY;

        }
        //Update image Components
        Image i = GetComponent<Image>();
        if (i)
        {
            i.sprite = ScriptableColorObject.sourceImage;
            i.color = ScriptableColorObject.imageColor;
            i.material = ScriptableColorObject.imageMaterial;
        }
        //Update text Components
        Text t = GetComponent<Text>();
        if (t)
        {
            t.font = ScriptableColorObject.primaryFont;
            t.color = ScriptableColorObject.textColor;
        }
    }

    //Adds an update button to ColorEditor
    [MenuItem("Update Stuff/updateButtonColor")]
    static void updateButtonColor()
    {
        foreach (MassColorChange itemsToChange in trackedObjects)
        {
            print("Hello");
            itemsToChange.ApplyColorChange();
        }
           
    }
}
