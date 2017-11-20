using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTutorialSelect : Objective {
    bool[] keys = new bool[2];
    void Start() {
        InitializeObjective();
        SetDescription("Use the \"Tab\" and \"LShift\" keys to cycle between units");
    }

	void Update () {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            keys[0] = true;
            CheckKey();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            keys[1] = true;
            CheckKey();
        }
    }

    private void CheckKey()
    {
        foreach (bool key in keys)
        {
            if (key == false)
            {
                return;
            }
        }
        SetState(ObjectiveState.Completed);
        StartCoroutine(DelayDisableObjective(2, true));
    }
}
