using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour {
    public Animator anim;
    public Animator turnAnimator;
    public Text titleText;
    public Text turnText;
    public Text turnCounter;
    public Text warningText;
    public Image line;
    public Color playerColor;
    public Color enemyColor;
    public GameObject warning;


    int amountTurns;
    int maxTurns;
    int totalActions;
    public bool isPlayerTurn;
    string text;

    public TurnSystem turnSystem;

    void Start () {
        amountTurns = 1;
        maxTurns = 10;
        text = "YOUR TURN";
        isPlayerTurn = true;
    }

	void Update () {
        totalActions = turnSystem.totalActions;
        
        //Ends turn if the enemy doesn't have any actions left
        if (!isPlayerTurn && totalActions <= 0)
        {
            pressEnd(true);
        }
        
    }

    public void pressEnd(bool forceEnd)
    {
        warning.SetActive(false);

        //If player has used all actions he is taken to the next turn
        if (totalActions <= 0 || !isPlayerTurn || forceEnd)
        {
            if (!isPlayerTurn)
            {
                text = "YOUR TURN";
                titleText.color = playerColor;
                line.color = playerColor;
            }
            else
            {
                text = "ENEMY TURN";
                titleText.color = enemyColor;
                line.color = enemyColor;
            }
            //Add all functionality here
            isPlayerTurn = !isPlayerTurn;
            turnAnimator.SetBool("isPlayerTurn", isPlayerTurn);
            titleText.text = text;
            turnText.text = text;

            anim.Play("turnFadeIn");

            turnSystem.resetActions(isPlayerTurn);
            turnSystem.displayAP(isPlayerTurn);

            if (isPlayerTurn)
                amountTurns++;

            if (amountTurns > maxTurns)
            {
                SceneManager.LoadScene("highScore");
            }
            else
            {
                turnCounter.text = amountTurns + "/" + maxTurns;
            }
        }
        else //Show warning if player has more than 0 actions
        {
            warning.SetActive(true);
            warningText.text = "Are you sure? You still have " + turnSystem.totalActions + " actions left";
        }
    }
    public void abortPress()
    {
        if (warning.activeSelf)
        {
            warning.SetActive(false);
        }
    }
}
