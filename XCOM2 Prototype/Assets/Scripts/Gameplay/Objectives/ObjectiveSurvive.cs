using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSurvive : Objective {

	void Start () {
        InitializeObjective();
        SetDescription("Survive " + mapConfig.turnSystem.maxTurns + " rounds");
    }
	
	void Update () {
        if (mapConfig.turnSystem.hud.amountTurns > mapConfig.turnSystem.maxTurns)
        {
            SetState(1);
        }
	}
}
