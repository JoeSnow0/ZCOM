using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<MonoBehaviour> eventSubscribedScripts = new List<MonoBehaviour>();
    public int gameEventID = 0;
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
#if UNITY_EDITOR
                //Warning for multiple Gamemanagers
                if (FindObjectsOfType<GameManager>().Length > 1)
                {
                    Debug.LogError("There is more than one game manager in the scene");
                }
#endif
            }
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void subscribeScriptToGameEventUpdates(MonoBehaviour pScript)
    {
        eventSubscribedScripts.Add(pScript);
    }

    //Remove duplicates
    public void desubscribeScriptToGameEventUpdates(MonoBehaviour pScript)
    {
        while (eventSubscribedScripts.Contains(pScript))
        {
            eventSubscribedScripts.Remove(pScript);
        }
    }

    public void playerPassedEvent()
    {
        gameEventID++;
        foreach(MonoBehaviour _script in eventSubscribedScripts)
        {
            _script.Invoke("gameEventUpdated", 0);
        }
    }

    // Update is called once per frame
    /*
	void Update ()
    {
		
	}
    */
}