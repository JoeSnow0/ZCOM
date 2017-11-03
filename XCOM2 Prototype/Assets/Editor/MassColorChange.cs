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
    
    void Start()
    {
        //Add ColorEditor to ScriptableColorObject
        if (ScriptableColorObject == null)
        {
            ScriptableColorObject = AssetDatabase.LoadAssetAtPath<ColorScriptableObject>("Assets/Scriptable Object/ColorEditor.asset");
        }
        trackedObjects.Add(this);
    }
    void Awake()
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

    public void ApplyColorChange(ColorScriptableObject cso = null)
    {
        ColorScriptableObject colorObject = ScriptableColorObject;
        if (cso != null)
            colorObject = cso;

        //Update button Components
        Button b = GetComponent<Button>();
        if (b)
        {

            ColorBlock block = b.colors;
            block.normalColor = colorObject.normalColor;
            block.highlightedColor = colorObject.highlightColor;
            block.pressedColor = colorObject.pressedColor;
            block.disabledColor = colorObject.disabledColor;
            block.colorMultiplier = colorObject.colorMultiplier;
            b.colors = block;

            //ButtonSettings button0 = colorObject.ButtonPresets[0];
            //ColorBlock block = b.colors;
            //block.normalColor = button0.normalColor;
            //block.highlightedColor = button0.highlightColor;
            //block.pressedColor = button0.pressedColor;
            //block.disabledColor = button0.disabledColor;
            //block.colorMultiplier = button0.colorMultiplier;
            //b.colors = block;

            return;
        }

        //Update outline Components
        Outline o = GetComponent<Outline>();
        if (o)
        {
            Vector2 shadow = o.effectDistance;
            o.effectColor = colorObject.effectColor;
            shadow.x = colorObject.effectDistanceX;
            shadow.y = colorObject.effectDistanceY;

        }
        //Update image Components
        Image i = GetComponent<Image>();
        if (i)
        {
            i.sprite = colorObject.sourceImage;
            i.color = colorObject.imageColor;
            i.material = colorObject.imageMaterial;
        }
        //Update text Components
        Text t = GetComponent<Text>();
        if (t)
        {
            t.font = colorObject.primaryFont;
            t.color = colorObject.textColor;
        }
    }

    //Update the list of objects with the Mass Color Change
    static public void UpdateButtonColor(ColorScriptableObject cso = null)
    {
        foreach (MassColorChange itemToChange in trackedObjects)
        {
            if (itemToChange == null)
                continue;

            itemToChange.ApplyColorChange(cso);
        }
           
    }
}
