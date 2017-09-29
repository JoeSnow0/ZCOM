using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuToggle : MonoBehaviour {

    public bool isPaused;
    public GameObject ingameMenu;
    private void Start()
    {
        isPaused = false;
    }
 
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ////Toggle menu
            //if (!isPaused)
            //{
            //    isPaused = true;
            //}
            //else
            //{
            //    isPaused = false;
            //}
            toggleMenu();
        }
    }
    public void toggleMenu()

    {
        isPaused = !isPaused;
        ingameMenu.SetActive(isPaused);
    }

}
