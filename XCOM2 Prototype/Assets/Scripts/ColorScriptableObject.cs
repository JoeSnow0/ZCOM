using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]

[CreateAssetMenu(fileName = "ColorEditor", menuName = "Class/CreateColorEditor", order = 2)]
public class ColorScriptableObject : ScriptableObject
{
    public Color primaryColor;
    public Color secondaryColor;
    public Color tertiaryColor;

    public Sprite primarySprite;
}





