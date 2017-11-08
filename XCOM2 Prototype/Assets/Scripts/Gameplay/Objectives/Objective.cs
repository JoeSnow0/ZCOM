using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour {

    protected Animator objectiveAnimatior;
    protected MapConfig mapConfig;
    protected Text descriptionText;

    public string description;

    protected void InitializeObjective()
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        objectiveAnimatior = GetComponentInChildren<Animator>();
        descriptionText = GetComponentInChildren<Text>();
    }

    public void SetState(int state)
    {
        objectiveAnimatior.SetInteger("progress", state);
    }

    public void SetDescription(string newDescription)
    {
        descriptionText.text = newDescription;
    }
}
