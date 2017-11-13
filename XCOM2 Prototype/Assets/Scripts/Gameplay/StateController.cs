using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{

    public enum GameState { TacticalMode, AttackMode };
    static public GameState CurrentState;
    public Animator panelAnimator;

    void start()
    {
        SetCurrentState(GameState.TacticalMode);
    }

    public void SetCurrentState(GameState setState)
    {
        CurrentState = setState;
        if (CurrentState == StateController.GameState.AttackMode)
        {
            panelAnimator.SetBool("displayPanel", true);
        }
        else
        {
            panelAnimator.SetBool("displayPanel", false);
        }

    }

    public bool CheckCurrentState(GameState CompareState)
    {
        if (CurrentState == CompareState)
        {

            return true;
        }
        else
        {
            return false;
        }
    }
}

