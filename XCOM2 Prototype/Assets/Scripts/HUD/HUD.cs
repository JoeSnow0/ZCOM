using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour {
    public GameObject alienUI;
    public GameObject playerUI;
    public GameObject endButton;
    Animator alienAnim;
    public Text turnCounter;
    public Text warningText;
    public Color playerColor;
    public Color enemyColor;
    public Color victoryColor;
    public Color defeatColor;
    public GameObject warning;


    int amountTurns;
    int maxTurns;
    int totalActions;
    public bool isPlayerTurn;

    public TurnSystem turnSystem;

    void Start () {
        amountTurns = 1;
        isPlayerTurn = true;
        
        maxTurns = turnSystem.getCurrentTurn(amountTurns); //Sets max turns and prints it out
        turnCounter.text = amountTurns + "/" + maxTurns;
        alienAnim = alienUI.GetComponent<Animator>();
    }

	void Update () {
        totalActions = turnSystem.totalActions;
        
        
        /*if (!isPlayerTurn && totalActions <= 0) //Ends turn if the enemy doesn't have any actions left
        {
            pressEnd(true);
        }*/

        //if(totalActions <= 0 && turnSystem.endTurn)
        //{
        //    pressEnd(true);
        //}
    }

    public void pressEnd(bool forceEnd)
    {
        warning.SetActive(false);

        if (totalActions <= 0 || !isPlayerTurn || forceEnd) //If player has used all actions he is taken to the next turn
        {
            isPlayerTurn = !isPlayerTurn;

            
            if (!isPlayerTurn)
            {
                playerUI.SetActive(false);
                alienUI.SetActive(true);
                alienAnim.Play("AlienActivityOn");
                turnSystem.spawnEnemy();
                endButton.SetActive(false);
            }
            else
            {
                playerUI.SetActive(true);
                alienUI.SetActive(false);
                endButton.SetActive(true);
            }
            //Add all functionality here, END TURN
            turnSystem.resetActions(isPlayerTurn);
            
            
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
