using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
    //public List<GameObject> objectsHit = new List<GameObject>();
    LineRenderer line;
    MapConfig mapConfig;
    public Vector3[] points;

	void Start () {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();

        line = GetComponent<LineRenderer>();
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DamageUnitsInSphere();
        }
        
        line.SetPosition(0, mapConfig.turnSystem.selectedUnit.transform.position);
        line.SetPosition(1, transform.position);
	}
    void DamageUnitsInSphere()
    {
        foreach(Collider other in Physics.OverlapSphere(transform.position, transform.localScale.x))
        {
            if (other.CompareTag("Unit") || other.CompareTag("FriendlyUnit"))
            {

                UnitConfig unitConfig = other.GetComponent<UnitConfig>();
                //unitConfig.health.TakeDamage(1, unitConfig.unitWeapon);
            }
        }
        
        
    }
}

