using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour {
    Animator anim;
    public Text titleText;
    public Text turnText;
    public Text turnCounter;
    public Color playerColor;
    public Color enemyColor;
    public GameObject warning;
    public Animator turnAnimator;
    int amountTurns;
    int maxTurns;
    int amountActions = 2;
    bool isPlayerTurn;
    string text;

    void Start () {
        anim = GetComponentInChildren<Animator>();
        amountTurns = 1;
        maxTurns = 10;
        text = "YOUR TURN";
        isPlayerTurn = true;
    }

	void Update () {
    }

    public void pressEnd(bool forceEnd)
    {
        warning.SetActive(false);


        //If player has used all actions he is taken to the next turn
        if (amountActions <= 0 || !isPlayerTurn || forceEnd)
        {
            if (!isPlayerTurn)
            {
                text = "YOUR TURN";
                titleText.color = playerColor;
            }
            else
            {
                text = "ENEMY TURN";
                titleText.color = enemyColor;
            }
            isPlayerTurn = !isPlayerTurn;
            turnAnimator.SetBool("isPlayerTurn", isPlayerTurn);
            titleText.text = text;
            turnText.text = text;
            anim.Play("turnFadeIn");


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
