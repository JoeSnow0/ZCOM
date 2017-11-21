using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTutorialCamera : Objective {

    bool[] keys = new bool[4];
    

    void Start() {
        InitializeObjective();
        SetDescription("Use WSAD to move the camera");
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.W))
        {
            keys[0] = true;
            CheckKey();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            keys[1] = true;
            CheckKey();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            keys[2] = true;
            CheckKey();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            keys[3] = true;
            CheckKey();
        }

    }

    private void CheckKey()
    {
        foreach(bool key in keys)
        {
            if(key == false)
            {
                return;
            }
        }
        SetState(ObjectiveState.Completed);
        StartCoroutine(DelayDisableObjective(2, true));
    }
}
