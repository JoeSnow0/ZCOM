using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class victoryCheck : MonoBehaviour {

    public GameObject gameEndCanvas;
    public GameObject gameEndText;
    public GameObject gameEndPanel;

    void Start()
    {
        //disable win screen at start
            gameEndCanvas.SetActive(false);
    }

    //For testing the win/lose screen
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && gameEndCanvas.activeInHierarchy == false)
        {
            winCheck(true);
        }
        if (Input.GetKeyDown(KeyCode.F) && gameEndCanvas.activeInHierarchy == false)
        {
            winCheck(false);
        }
    }
    //Call if win/lose conditions have been met
	public void winCheck(bool hasWon)
    {
        gameEndCanvas.SetActive(true);
        if (hasWon == false)
        {
            gameEndText.GetComponent<Text>().text = "Defeated!";
            gameEndPanel.GetComponent<Image>().color = Color.red;
        }
    }
}
