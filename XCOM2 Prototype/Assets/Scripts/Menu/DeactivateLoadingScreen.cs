using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateLoadingScreen : MonoBehaviour {

    [SerializeField]
    GameObject loadingScreen;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.anyKey)
        {
            loadingScreen.SetActive(false);
        }
	}
}
