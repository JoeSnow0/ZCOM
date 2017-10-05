using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuButtons : MonoBehaviour
{

    static List<GameObject> sceneStates = new List<GameObject>();
    public GameObject mainMenu;
    public GameObject highScore;
    public GameObject credits;
    public static int scene;

    private void Awake()
    {

        mainMenu.SetActive(true);
        if (scene == 2)
        {
            OnHighScoreClick();
        }


    }

    public void OnButtonPress(string sceneName)
    {



        if (sceneName == "Exit")
        {
            Application.Quit();
        }
        else if (sceneName == "gameSession")
        {
            SceneManager.LoadScene(sceneName);
        }
        else if (sceneName == "mainMenu")
        {
            SceneManager.LoadScene(sceneName);
        }
        else if (sceneName == "highScore")
        {
            scene = 2;
            SceneManager.LoadScene("mainMenu");
        }

    }

    public void OnMainMenuClick()
    {
        scene = 1;
        DeactivatePanel();
        mainMenu.SetActive(true);
    }
    public void OnHighScoreClick()
    {
        scene = 2;
        DeactivatePanel();
        highScore.SetActive(true);
    }
    public void OnCreditsClick()
    {
        scene = 3;
        DeactivatePanel();
        credits.SetActive(true);
    }



    //    public void onButtonPress(int panelIndex)
    //    {
    //        deactivatePanel();

    //        if (panelIndex == 0)
    //        {
    //            SceneManager.LoadScene("gameSession");
    //        }
    //        for (int i = 1; i < sceneStates.Count; i++)
    //        {
    //            if (panelIndex == i)
    //            {
    //                sceneStates[i].SetActive(true);
    //            }
    //            else
    //            {
    //                Application.Quit();
    //            }
    //        }



    //    }

    void DeactivatePanel()
    {
        mainMenu.SetActive(false);
        highScore.SetActive(false);
        credits.SetActive(false);
    }
    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
