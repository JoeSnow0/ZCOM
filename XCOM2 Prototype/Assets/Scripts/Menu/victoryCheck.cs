using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class victoryCheck : MonoBehaviour {

    public MapConfig mapConfig;
    public GameObject gameEndCanvas;
    public Text gameEndText;
    public GameObject gameEndPanel;
    public GameObject markers;
    
    void Start()
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        //disable win screen at start
        gameEndCanvas.SetActive(false);
    }

    //Call if win/lose conditions have been met
	public void winCheck(bool hasWon)
    {
        markers.SetActive(false);
        gameEndCanvas.SetActive(true);
        if (hasWon == false)
        {
            gameEndText.text = "DEFEAT";
            gameEndText.color = mapConfig.turnSystem.defeatColor;
        }
        else
        {
            gameEndText.color = mapConfig.turnSystem.victoryColor;
        }
    }
}
