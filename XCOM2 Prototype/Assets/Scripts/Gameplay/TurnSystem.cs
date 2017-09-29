using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour {
    public GameObject[] allUnits;
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    public int totalActions;

    public Unit selectedUnit;

    void Start () {
        allUnits = GameObject.FindGameObjectsWithTag("Unit");

        for (int i = 0; i < allUnits.Length; i++)
        {
            if (allUnits[i].GetComponent<Unit>().isFriendly)
            {
                playerUnits.Add(allUnits[i].GetComponent<Unit>());
            }
            else
            {
                enemyUnits.Add(allUnits[i].GetComponent<Unit>());
            }
        }

        totalActions = playerUnits.Count * 2;

        selectedUnit = playerUnits[0];
        selectedUnit.isSelected = true;

        displayAP(true);
    }

	void Update () {
        selectUnit();
        attackUnit();
	}
    public void displayAP(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].animUI.SetBool("display", true);
            }
            for(int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].animUI.SetBool("display", false);
            }
        }
        else
        {
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].animUI.SetBool("display", false);
            }
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].animUI.SetBool("display", true);
            }
        }
    }

    public void selectUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<Unit>())
                {
                    if (hit.collider.GetComponent<Unit>().isFriendly)
                    {
                        selectedUnit.GetComponent<Unit>().isSelected = false;
                        selectedUnit = hit.collider.GetComponent<Unit>();
                        selectedUnit.GetComponent<Unit>().isSelected = true;
                    }
                }
            }
        }
    }

    void attackUnit()
    {
        if (Input.GetMouseButtonDown(0) && selectedUnit.actions > 1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<Unit>())//Checks if it hits an enemy
                {
                    Unit target = hit.collider.GetComponent<Unit>();
                    if (!target.isFriendly)
                    {
                        target.TakeDamage(selectedUnit.damage);
                        totalActions -= selectedUnit.actions;
                        selectedUnit.actions = 0;
                        selectNextUnit();
                    }
                }
            }
        }
    }

    public void resetActions(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            totalActions = playerUnits.Count * 2;
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].actions = 2;
            }
        }
        else
        {
            totalActions = enemyUnits.Count * 2;
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].actions = 2;
            }
        }
    }
    public void selectNextUnit()
    {
        for(int i = 0; i < playerUnits.Count; i++)
        {
            if(playerUnits[i].actions > 0)
            {
                selectedUnit.GetComponent<Unit>().isSelected = false;
                selectedUnit = playerUnits[i];
                selectedUnit.GetComponent<Unit>().isSelected = true;
            }
        }
    }
}
