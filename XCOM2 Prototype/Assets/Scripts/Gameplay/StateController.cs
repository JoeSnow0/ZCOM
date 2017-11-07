using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{

    public enum GameState { TacticalMode, AttackMode };
    public GameState CurrentState;

    void start()
    {
        SetCurrentState(GameState.TacticalMode);
    }

    public void SetCurrentState(GameState state)
    {
        CurrentState = state;
    }

    public GameState CheckCurrentState()
    {
        return CurrentState;
    }
}

