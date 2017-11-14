using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuToggle : MonoBehaviour {

    public bool isPaused;
    public bool optionsToggle;
    public GameObject ingameMenu;
    public GameObject ingameOptions;
    public ManagerConfig managerConfig;

    private void Start()
    {
        managerConfig = GetComponent<ManagerConfig>();
        isPaused = false;
        optionsToggle = false;
    }
    //hide options/ingame menu depending on what's currently visible
    void Update()
    {
        if (Input.GetKeyDown(managerConfig.mapConfig.turnSystem.leaveAttackMode) && managerConfig.mapConfig.stateController.CheckCurrentState(StateController.GameState.TacticalMode))
        {
            if (optionsToggle == false)
            {

                toggleMenu();
            }
            if (optionsToggle == true)
            {

                ToggleOptions();
            }
            
        }
    }
    
    //Show/hide menu
    public void toggleMenu()

    {
        isPaused = !isPaused;
        ingameMenu.SetActive(isPaused);
    }
    //Show/hide Options
    public void ToggleOptions()
    {
        ingameMenu.SetActive(!isPaused);
        optionsToggle = !optionsToggle;
        ingameOptions.SetActive(optionsToggle);

        if (!optionsToggle)
        {
            ingameMenu.SetActive(isPaused);
        }
    }
}
