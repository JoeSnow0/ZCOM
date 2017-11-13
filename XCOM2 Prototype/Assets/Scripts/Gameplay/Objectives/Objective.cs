using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour {

    protected Animator objectiveAnimatior;
    protected MapConfig mapConfig;
    protected Text descriptionText;
    protected string description;
    protected int currentState;
    protected bool isBonus;
    protected Objective objectiveController;

    private victoryCheck victoryScript;
    [SerializeField]private List<Objective> objectives = new List<Objective>();

    private void Awake()
    {
        AddActiveObjectives(objectives);

        victoryScript = FindObjectOfType<victoryCheck>();
    }

    private void Update()
    {
        
    }

    protected void InitializeObjective(int state = 0, bool bonus = false)
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        objectiveAnimatior = GetComponentInChildren<Animator>();
        descriptionText = GetComponentInChildren<Text>();
        currentState = state;
        isBonus = bonus;
        SetState(state);
    }
    private void AddActiveObjectives(List<Objective> allObjectives)
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

    private void AddObjective(Objective objective)
    {
        objectives.Add(objective);
        objective.objectiveController = this;
    }

    private void CheckObjectives(List<Objective> objectiveList)
    {
        bool won = true;
        foreach (Objective objective in objectiveList)
        {
            if(!objective.isBonus && objective.currentState == 0)
            {
                won = false;
            }
            if (!won)
                return;
        }
        if (won)
        {
            victoryScript.winCheck(true);
        }
    }

    protected void SetState(int state)
    {
        currentState = state;
        objectiveAnimatior.SetInteger("progress", currentState);
        CheckObjectives(objectiveController.objectives);
    }

    protected void SetDescription(string newDescription)
    {
        description = newDescription;
        descriptionText.text = "";
        StartCoroutine(PlayText(description, descriptionText));
    }

    protected IEnumerator PlayText(string inputText, Text outputText)
    {
        float waitDuration = 1f / inputText.Length;
        foreach (char c in inputText)
        {
            outputText.text += c;
            yield return new WaitForSeconds(waitDuration);
        }
    }

    private void SetController()
    {

    }
}
