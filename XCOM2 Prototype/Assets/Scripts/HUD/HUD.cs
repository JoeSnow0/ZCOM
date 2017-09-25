using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour {
    Animator anim;
    public Text titleText;
    public Text turnCounter;
    public Color playerColor;
    public Color enemyColor;
    int amountTurns;
    int maxTurns;
    bool isPlayerTurn;
    

    void Start () {
        anim = GetComponentInChildren<Animator>();
        amountTurns = 1;
        maxTurns = 10;
	}

	void Update () {
    }

    public void pressEnd()
    {
        if (isPlayerTurn)
        {
            titleText.text = "YOUR TURN";
            titleText.color = playerColor;
        }
        else
        {
            titleText.text = "ENEMY TURN";
            titleText.color = enemyColor;
        }
        anim.Play("turnFadeIn");
        isPlayerTurn = !isPlayerTurn;
        
        if (!isPlayerTurn)
            amountTurns++;

        if (amountTurns > maxTurns)
        {
            //SceneManager.LoadScene(highScore);
        }
        else
        {
            
            turnCounter.text = amountTurns + "/" + maxTurns;
        }
    }
}
