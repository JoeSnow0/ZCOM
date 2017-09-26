using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    // Use this for initialization
   [Range(0, 100)]
   public int health = 100;
   int MAX_HEALTH = 100;
   int MIN_HEALTH = 0;
    

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
