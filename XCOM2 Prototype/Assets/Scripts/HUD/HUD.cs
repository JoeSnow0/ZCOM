using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    public Color victoryColor;
    public Color defeatColor;
    public GameObject warning;


    int amountTurns;
    int maxTurns;
    int totalActions;
    public bool isPlayerTurn;
    string text;

    public TurnSystem turnSystem;

    void Start () {
        amountTurns = 1;
        text = "YOUR TURN";
        isPlayerTurn = true;
        
        maxTurns = turnSystem.getCurrentTurn(amountTurns); //Sets max turns and prints it out
        turnCounter.text = amountTurns + "/" + maxTurns;
    }

	void Update () {
        totalActions = turnSystem.totalActions;
        
        
        if (!isPlayerTurn && totalActions <= 0) //Ends turn if the enemy doesn't have any actions left
        {
            pressEnd(true);
        }
        
    }

    public void pressEnd(bool forceEnd)
    {
        warning.SetActive(false);

        if (totalActions <= 0 || !isPlayerTurn || forceEnd) //If player has used all actions he is taken to the next turn
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
                turnSystem.spawnEnemy();
            }
            //Add all functionality here, END TURN
            isPlayerTurn = !isPlayerTurn;
            turnAnimator.SetBool("isPlayerTurn", isPlayerTurn);
            titleText.text = text;
            turnText.text = text;

            anim.Play("turnFadeIn");

            turnSystem.resetActions(isPlayerTurn);
            turnSystem.displayAP(isPlayerTurn);
            
            
            if (isPlayerTurn)
            {
                turnSystem.selectNextUnit();
            }

            if (isPlayerTurn)
                amountTurns++;

            maxTurns = turnSystem.getCurrentTurn(amountTurns); //Sets max turns and sends current turn to turn system
            


            if (amountTurns <= maxTurns) //Displays VICTORY instead of the turn if the player won
                turnCounter.text = amountTurns + "/" + maxTurns;
            else
            {
                turnCounter.text = "VICTORY";
                turnCounter.color = victoryColor;
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
