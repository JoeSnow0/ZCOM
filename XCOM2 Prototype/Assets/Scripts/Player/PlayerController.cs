using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    Camera cam;

    public LayerMask groundLayer;

    public NavMeshAgent playerAgent;

    #region Monobehavior API

    // Use this for initialization
    void Awake () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonDown(1))
        {
            playerAgent.SetDestination(GetPointUnderCursor());
        }	
	}
    #endregion

    private Vector3 GetPointUnderCursor()
    {
        Vector2 screenPosition = Input.mousePosition;
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(screenPosition);

        RaycastHit hitPosition;

        Physics.Raycast(mouseWorldPosition, cam.transform.forward, out hitPosition, 100, groundLayer);

        return hitPosition.point;
    }
}
