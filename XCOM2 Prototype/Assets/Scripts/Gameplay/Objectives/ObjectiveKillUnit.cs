using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveKillUnit : Objective {
    [SerializeField] private int killRequired;
    [SerializeField] ClassStatsObject unitType;

	void Start() {
        InitializeObjective();

        if (mapConfig.turnSystem.killedUnits.ContainsKey(unitType.unitClassName))
        {
            killRequired = mapConfig.turnSystem.killedUnits[unitType.unitClassName] + killRequired;
        }

        //Checks if any units have previously been killed, if remove them from the counter, adds 's' when plural
        SetDescription("Kill " + (killRequired - mapConfig.turnSystem.GetKillCount(unitType)) + " " + unitType.unitClassName + 
            ((killRequired - mapConfig.turnSystem.GetKillCount(unitType) > 1) ? "s" : ""));
    }
	
	void Update() {
		if(mapConfig.turnSystem.killedUnits.ContainsKey(unitType.unitClassName) && mapConfig.turnSystem.killedUnits[unitType.unitClassName] >= killRequired)
        {
            SetState(ObjectiveState.Completed);
        }
	}
}
