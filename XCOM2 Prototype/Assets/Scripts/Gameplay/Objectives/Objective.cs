﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour {
    protected enum ObjectiveState { InProgress, Completed, Failed};
    
    
    protected ObjectiveState objectiveState;
    protected Animator objectiveAnimatior;
    protected MapConfig mapConfig;
    protected Text descriptionText;
    protected string description;
    protected bool isBonus;
    protected Objective objectiveController;

    [SerializeField] private bool isTutorial = false;
    private victoryCheck victoryScript;
    private List<Objective> objectives = new List<Objective>();
    private int currentObjectiveIndex = 0;

    private void Awake()
    {
        AddActiveObjectives(objectives);

        victoryScript = FindObjectOfType<victoryCheck>();
    }

    private void Update()
    {

    }

    protected void InitializeObjective(bool bonus = false, ObjectiveState state = ObjectiveState.InProgress)// Initializes objectives that inherit variables, this code is never run in the objective controller
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        objectiveAnimatior = GetComponentInChildren<Animator>();
        descriptionText = GetComponentInChildren<Text>();
        isBonus = bonus;
        SetState(state);
    }
    private void AddActiveObjectives(List<Objective> allObjectives)// Adds all objectives to a list
    {
        foreach (Objective objective in GetComponentsInChildren<Objective>(true))
        {
            if (objective.transform != transform)
            {
                allObjectives.Add(objective);
                objective.objectiveController = this;
            }
        }
    }

    private void AddObjective(Objective objective)// Adds new objectives, currently not used
    {
        objectives.Add(objective);
        objective.objectiveController = this;
    }

    private void CheckObjectives(List<Objective> objectiveList)// Checks if the player has completed all objectives, if they have they win
    {
        bool won = true;
        foreach (Objective objective in objectiveList)
        {
            if(objective.gameObject.activeSelf && !objective.isBonus && objective.objectiveState == ObjectiveState.InProgress)
            {
                won = false;
            }
            if (!won)
                return;
        }

        if (won)
        {
            if(!isTutorial)
                victoryScript.winCheck(true);
        }
    }

    protected void SetState(ObjectiveState state)// Used to set the state of an objective, 0 = in progress, 1 = complete, 2 = failed
    {
        objectiveState = state;
        objectiveAnimatior.SetInteger("progress", (int)objectiveState);
        objectiveController.CheckObjectives(objectiveController.objectives);
    }

    protected void SetDescription(string newDescription)// Changes the description, has a built in typewriter effect to it
    {
        description = newDescription;
        descriptionText.text = "";
        StartCoroutine(PlayText(description, descriptionText));
    }

    protected void DisableObjective(List<Objective> objectiveList)// Disables the objective
    {
        objectiveList.Remove(this);
        gameObject.SetActive(false);
        if (objectiveController.currentObjectiveIndex >= objectiveController.transform.childCount - 1)
        {
            objectiveController.transform.parent.gameObject.SetActive(false);
        }
    }

    protected IEnumerator PlayText(string inputText, Text outputText)// Typewriter effect
    {
        float waitDuration = 1f / inputText.Length;
        foreach (char c in inputText)
        {
            outputText.text += c;
            yield return new WaitForSeconds(waitDuration);
        }
    }

    protected IEnumerator DelayDisableObjective(float time, bool enableNext = false)
    {
        yield return new WaitForSeconds(time);
        DisableObjective(objectives);
        if (enableNext)
        {
            objectiveController.EnableObjective();
        }
    }

    public List<Objective> GetObjectives()
    {
        return objectives;
    }

    private void EnableObjective()
    {
        currentObjectiveIndex++;
        if(currentObjectiveIndex < transform.childCount)
        {
            transform.GetChild(currentObjectiveIndex).gameObject.SetActive(true);
        }
    }

    private void SetController()// Unused
    {

    }
}
