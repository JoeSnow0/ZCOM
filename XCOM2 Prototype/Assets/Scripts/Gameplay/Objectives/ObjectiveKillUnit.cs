using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveKillUnit : Objective {
    public int killRequired;
	void Start() {
        InitializeObjective();
        SetDescription("Kill " + killRequired + " zombies");
    }
	
	void Update () {
		if(mapConfig.turnSystem.killCount >= killRequired)
        {
            SetState(1);
        }
	}
}
