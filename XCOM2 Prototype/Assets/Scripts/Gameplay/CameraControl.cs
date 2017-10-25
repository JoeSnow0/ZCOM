using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [Header("X Position")]
    [Tooltip("Min and Max values for the cameras X Position")]
    public float xPosMin;
    public float xPosMax;
    [Header("Y Position")]
    [Tooltip("Min and Max values for the cameras y Position")]
    public float yPosMin;
    public float yPosMax;
    [Header("Z Position")]
    [Tooltip("Min and Max values for the cameras z Position")]
    public float zPosMin;
    public float zPosMax;
    [Header("Speed")]
    [Tooltip("Speed for camera rotation")]
    [SerializeField]
    [RangeAttribute(0f,100f)]
    float moveSpeed;
    [Tooltip("Speed for camera movement")]
    [SerializeField]
    [RangeAttribute(0f, 100f)]
    float rotSpeed;
    [Tooltip("Speed for camera zoom")]
    [SerializeField]
    [RangeAttribute(0, 100)]
    int zoomSpeed;
    [Header("Camera Target")]
    public GameObject CameraTarget;
    public GameObject Camera;

    [Header("Camera Keybindings")]
    [SerializeField]    string cameraZoom;

    [SerializeField]    KeyCode cameraMoveLeft;
    [SerializeField]    KeyCode cameraMoveRight;
    [SerializeField]    KeyCode cameraMoveForward;
    [SerializeField]    KeyCode cameraMoveBackward;
    [SerializeField]    KeyCode cameraRotateLeft;
    [SerializeField]    KeyCode cameraRotateRight;
    Vector3 targetPosition;
    Vector3 startPosition;
    float moveToTargetLerp = 0;
    float rotateLerp = 1;
    Vector3 targetRotation;
    int yRotation = 45;
    float currentScroll = -1;
    bool movingCamera = false;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        //Move left and right
        if (CameraTarget.transform.position.x >= xPosMin && CameraTarget.transform.position.x <= xPosMax)
        {
            if (Input.GetKey(cameraMoveLeft) && CameraTarget.transform.position.x >= xPosMin)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(cameraMoveRight) && CameraTarget.transform.position.x <= xPosMax)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            } 
        }



        //reset movement within boundries
        if (CameraTarget.transform.position.x > xPosMax && CameraTarget.transform.position.x < xPosMin)
        {
            Vector3 p = CameraTarget.transform.position;
            CameraTarget.transform.position = new Vector3(Mathf.Clamp(p.x, xPosMin, xPosMax), p.y, p.z);
        }
        

        //Move forwards and backwards
        if (CameraTarget.transform.position.z >= zPosMin && CameraTarget.transform.position.z <= zPosMax)
        {
            if (Input.GetKey(cameraMoveBackward) && CameraTarget.transform.position.z >= zPosMin)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(cameraMoveForward) && CameraTarget.transform.position.z <= zPosMax)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            
        }
        //reset movement within boundries
        if (CameraTarget.transform.position.z > zPosMax && CameraTarget.transform.position.z < zPosMin)
        {
            Vector3 p = CameraTarget.transform.position;
            CameraTarget.transform.position = new Vector3(p.x, p.y, Mathf.Clamp(p.z, zPosMin, zPosMax));
        }


        //Rotate camera
        if (Input.GetKeyDown(cameraRotateLeft))
        {
            yRotation += 90;
            targetRotation = new Vector3(0, yRotation, 0);
            rotateLerp = 0;
        }
        if (Input.GetKeyDown(cameraRotateRight))
        {
            yRotation -= 90;
            targetRotation = new Vector3(0, yRotation, 0);
            rotateLerp = 0;
        }
        if (rotateLerp < 1)
        {
            rotateLerp += Time.deltaTime;
            CameraTarget.transform.rotation = Quaternion.Lerp(CameraTarget.transform.rotation, Quaternion.Euler(targetRotation), Mathf.SmoothStep(0, 1, rotateLerp));
        }

        
        //Zoom in and out
        if (Input.GetAxis(cameraZoom) > 0 && currentScroll < 1)
        {
            currentScroll += Input.GetAxis(cameraZoom);
            Camera.transform.position += Camera.transform.forward * Input.GetAxis(cameraZoom) * zoomSpeed;
        }
        else if (Input.GetAxis(cameraZoom) < 0 && currentScroll > -1)
        {
            currentScroll += Input.GetAxis(cameraZoom);
            Camera.transform.position += Camera.transform.forward * Input.GetAxis(cameraZoom) * zoomSpeed;
        }

        //Moves camera to selected unit
        if (moveToTargetLerp <= 1 && movingCamera)
        {
            moveToTargetLerp += Time.deltaTime / 0.5f;
            CameraTarget.transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, moveToTargetLerp));
        }
    }
    //Move the camera to a specific position within the selected time
    public void MoveToTarget(Vector3 selectedPosition, float time)
    {
        movingCamera = true;
        moveToTargetLerp = time;
        startPosition = CameraTarget.transform.position;
        targetPosition = selectedPosition;
    }
}
