using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour {
    public GameObject projectile;
    public Transform projectileStart;

    void Start () {
        
	}
	
	void Update () {
		
	}

    public void Shoot()
    {
        //Instantiate(projectile, projectileStart.position, Quaternion.Euler(0, projectileStart.eulerAngles.y, projectileStart.eulerAngles.z));
    }
}
