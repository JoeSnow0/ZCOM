using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour {

    [SerializeField] int healAmount = 2;

    [SerializeField] WeaponInfoObject medkit;

    Health health;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.H))
        {
            health.Healing(healAmount, medkit);
        }
	}
}
