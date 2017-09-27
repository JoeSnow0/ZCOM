using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor : MonoBehaviour {

    public Material hoverCursor;
    // Update is called once per frame
    Camera cam;
    public Material orgMat;
    public Material hoverMat;
    public Color muoseHoverColor;
    private void Awake()
    {
        cam = Camera.main;

    }
    void Update () {
       
            GetPointUnderCursor();
        
    }

    private void GetPointUnderCursor()
    {
        
        Vector3 screenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(screenPosition);

        RaycastHit hitPosition;

        Physics.Raycast(mouseWorldPosition, cam.transform.forward, out hitPosition, 100);
        if (hitPosition.collider)
        {
            if (hitPosition.collider.CompareTag("Ground"))
            {
                
                GameObject hit = hitPosition.collider.gameObject;
                hit.GetComponent<squarefunctions>().currentColor = muoseHoverColor;

            }
        }
    }
}
