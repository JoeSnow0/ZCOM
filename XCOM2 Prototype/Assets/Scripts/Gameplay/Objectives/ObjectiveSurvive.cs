using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSurvive : Objective {
    [SerializeField] private int maxTurns;

    void Start() {
        InitializeObjective();

        if(maxTurns < 1)
            maxTurns = mapConfig.turnSystem.maxTurns;
        
        SetDescription("Survive " + maxTurns + (maxTurns > 1 ? " rounds" : " round"));

    }

	void Update() {
        if(mapConfig.turnSystem.hud.amountTurns > maxTurns)
        {
            SetState(ObjectiveState.Completed);
        }
	}
}
