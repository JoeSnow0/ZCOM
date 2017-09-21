using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class creditsButtons : MonoBehaviour {

    public void onButtonpress(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
