using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    // Use this for initialization
    int health;
    [SerializeField, Range(0, 100)]
    int MAX_HEALTH = 100;
    int MIN_HEALTH = 0;
    
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            // Die
        }
    }

	void Start () {
        health = MAX_HEALTH;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
