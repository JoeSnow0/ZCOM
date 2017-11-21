using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTutorialTarget : Objective {

    void Start() {
        InitializeObjective();
        SetDescription("Press '1' to start targeting, target with 'Tab' and 'LShift' then press '1' again to shoot");
    }

	void Update () {
        if(TurnSystem.hasShot)
        {
            SetState(ObjectiveState.Completed);
            StartCoroutine(DelayDisableObjective(2));
        }
	}
}
