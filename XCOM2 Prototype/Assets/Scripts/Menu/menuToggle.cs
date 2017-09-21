using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuToggle : MonoBehaviour {

    public bool isPaused = false;
    public GameObject ingameMenu;
    
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Toggle menu
            if (!isPaused)
            {
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }
            toggleMenu();
        }
    }
    void toggleMenu()

    {
        ingameMenu.SetActive(isPaused);
        //Pause and unpause the gameplay
        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else if (!isPaused)
        {
            Time.timeScale = 1;
        }
        
    }

}
