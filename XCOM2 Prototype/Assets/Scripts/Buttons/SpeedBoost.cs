using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBoost : MonoBehaviour {
    static bool isBoosted = false;
    
    public Color normal;
    public Color boostColor;
    Text text;
    private void Start()
    {
        
    }
    public void onSpeedBoost(Outline speedButton)
    {
        if(text == null)
            text = speedButton.GetComponentInChildren<Text>();
        isBoosted = !isBoosted;
        if (isBoosted)
        {
            UnitConfig.animaitionSpeed = UnitConfig.normalSpeed * 2;
            speedButton.effectColor = boostColor;
            text.color = boostColor;
        }
        else
        {
            UnitConfig.animaitionSpeed = UnitConfig.normalSpeed;
            speedButton.effectColor = normal;
            text.color = normal;
        }
    }
}
