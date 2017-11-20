using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTutorialMove : Objective {

    void Start() {
        InitializeObjective();
        SetDescription("Right-click on a tile to move the selected character");
    }

	void Update () {
        if (TurnSystem.hasMoved)
        {
            SetState(ObjectiveState.Completed);
            StartCoroutine(DelayDisableObjective(2, true));
        }
	}
}
