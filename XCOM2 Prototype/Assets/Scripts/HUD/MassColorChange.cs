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

            //return;
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
    static public void updateButtonColor(ColorScriptableObject cso = null)
    {
        foreach (MassColorChange itemsToChange in trackedObjects)
        {
            itemsToChange.ApplyColorChange(cso);
        }
           
    }
}
