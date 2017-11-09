using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour {

    protected Animator objectiveAnimatior;
    protected MapConfig mapConfig;
    protected Text descriptionText;
    protected string description;

    [SerializeField]private List<Objective> objectives = new List<Objective>();

    private void Start()
    {
        foreach(Objective objective in GetComponentsInChildren<Objective>())
        {
            if (objective.transform != transform)
            {
                objectives.Add(objective);
            }
        }
        
    }

    private void Update()
    {
        
    }

    protected void InitializeObjective()
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        objectiveAnimatior = GetComponentInChildren<Animator>();
        descriptionText = GetComponentInChildren<Text>();
    }

    protected void SetState(int state)
    {
        objectiveAnimatior.SetInteger("progress", state);
    }

    protected void SetDescription(string newDescription)
    {
        descriptionText.text = newDescription;
    }
}
