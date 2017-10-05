using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuButtons : MonoBehaviour {


    public void onButtonPress(string sceneName)
    {
        if(sceneName == "Exit")
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
        
    }
    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
