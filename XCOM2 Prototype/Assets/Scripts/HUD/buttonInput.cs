using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class buttonInput : MonoBehaviour {
    public KeyCode useAbility;
  
	void Update ()
    {
       if (Input.GetKeyDown(useAbility))
        {
            Debug.Log("IT WORKS WOW!");
        }
	}
}
