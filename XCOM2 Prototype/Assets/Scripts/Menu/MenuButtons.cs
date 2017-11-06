using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuButtons : MonoBehaviour
{

    static List<GameObject> sceneStates = new List<GameObject>();

    public GameObject mainMenu, highScore, credits, loadingScreen;
    public static int scene;
    static MusicController musicController;

    
    private void Awake()
    {
        if (scene == 2)
        {
            OnHighScoreClick();
        }
    }
    //Menu Navigation
    public void OnButtonPress(string sceneName)
    {


        if (sceneName == "Exit")
        {
            Application.Quit();
        }
        else if (sceneName == "gameSession")
        {
            DeactivatePanel();
            loadingScreen.SetActive(true);
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
    //Prepare the main menu, hide other panels
    public void OnMainMenuClick()
    {
        scene = 1;
        DeactivatePanel();
        mainMenu.SetActive(true);
    }
    //Prepare the highscore, hide other panels
    public void OnHighScoreClick()
    {
        scene = 2;
        DeactivatePanel();
        highScore.SetActive(true);
    }
    //Prepare the credits, hide other panels
    public void OnCreditsClick()
    {
        scene = 3;
        DeactivatePanel();
        credits.SetActive(true);
    }

    //Hides other panels in the main menu
    void DeactivatePanel()
    {
        mainMenu.SetActive(false);
        highScore.SetActive(false);
        credits.SetActive(false);
        loadingScreen.SetActive(false);
    }
    //reload current scene
    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
