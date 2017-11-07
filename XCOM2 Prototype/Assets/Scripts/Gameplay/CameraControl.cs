using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [Header("X Position")]
    [Tooltip("Min and Max values for the cameras X Position")]
    public float xPosMin;
    float xPosMax;
    [Header("Y Position")]
    [Tooltip("Min and Max values for the cameras y Position")]
    public float yPosMin;
    public float yPosMax;
    [Header("Z Position")]
    [Tooltip("Min and Max values for the cameras z Position")]
    public float zPosMin;
    float zPosMax;

    [Header("Speed")]
    [Tooltip("Speed for camera speed")]
    [SerializeField]
    [RangeAttribute(0f,100f)]
    float moveSpeed;
    [Tooltip("Speed for camera rotation")]
    [SerializeField]
    [RangeAttribute(0f, 10f)]
    private float rotationSpeed;
    [Tooltip("Speed for camera zoom")]
    [SerializeField]
    [RangeAttribute(0, 100)]
    int zoomSpeed;

    [Header("Rotation Curve")]
    public AnimationCurve rotationCurve;

    [Header("Camera Target")]
    public GameObject cameraTarget;
    public GameObject cameraHolder;

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
    bool movingCamera = false;
    //private Vector3 velocity = Vector3.zero;

    MapConfig mapConfig;
    public bool playerMovedCamera;

    private void Start()
    {
        //Add the map incase its missing
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        xPosMax = mapConfig.tileMap.mapSizeX * mapConfig.tileMap.offset;
        zPosMax = mapConfig.tileMap.mapSizeY * mapConfig.tileMap.offset;
    }

    void Update()
    {
        //Move left and right
        if (cameraTarget.transform.position.x >= xPosMin && cameraTarget.transform.position.x <= xPosMax)
        {
            if (Input.GetKey(cameraMoveLeft) && cameraTarget.transform.position.x >= xPosMin)
            {
                movingCamera = false;
                if (!mapConfig.turnSystem.playerTurn)
                {
                    playerMovedCamera = true;
                }
                cameraTarget.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(cameraMoveRight) && cameraTarget.transform.position.x <= xPosMax)
            {
                movingCamera = false;
                if (!mapConfig.turnSystem.playerTurn)
                {
                    playerMovedCamera = true;
                }
                cameraTarget.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            } 
        }

        //reset movement within boundries
        if (cameraTarget.transform.position.x >= xPosMax || cameraTarget.transform.position.x <= xPosMin)
        {
            Vector3 p = cameraTarget.transform.position;
            cameraTarget.transform.position = new Vector3(Mathf.Clamp(p.x, xPosMin, xPosMax), p.y, p.z);
        }
        

        //Move forwards and backwards
        if (cameraTarget.transform.position.z >= zPosMin && cameraTarget.transform.position.z <= zPosMax)
        {
            if (Input.GetKey(cameraMoveBackward) && cameraTarget.transform.position.z >= zPosMin)
            {
                movingCamera = false;
                if (!mapConfig.turnSystem.playerTurn)
                {
                    playerMovedCamera = true;
                }
                cameraTarget.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(cameraMoveForward) && cameraTarget.transform.position.z <= zPosMax)
            {
                movingCamera = false;
                if (!mapConfig.turnSystem.playerTurn)
                {
                    playerMovedCamera = true;
                }
                cameraTarget.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
        }

        //reset movement within boundries
        if (cameraTarget.transform.position.z >= zPosMax || cameraTarget.transform.position.z <= zPosMin)
        {
            Vector3 p = cameraTarget.transform.position;
            cameraTarget.transform.position = new Vector3(p.x, p.y, Mathf.Clamp(p.z, zPosMin, zPosMax));
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
            rotateLerp += Time.deltaTime * rotationSpeed;
            cameraTarget.transform.rotation = Quaternion.Lerp(cameraTarget.transform.rotation, Quaternion.Euler(targetRotation), rotationCurve.Evaluate(rotateLerp));
        }

        //HACK: Needs a better way of restricting movement of the zoom
        //Zoom in and out
        if (Input.GetAxis(cameraZoom) != 0)
        {
            cameraHolder.transform.localPosition += cameraHolder.transform.forward * Input.GetAxis(cameraZoom) * zoomSpeed;
            Vector3 p = cameraHolder.transform.localPosition;
            cameraHolder.transform.localPosition = new Vector3(0, Mathf.Clamp(p.y, yPosMin, yPosMax), -5);
        }
        ////reset within bounds
        //if (cameraHolder.transform.localPosition.y >= yPosMax || cameraHolder.transform.localPosition.y <= yPosMin)
        //{
            
            
        //}

        //Moves camera to selected unit
        if (movingCamera && !playerMovedCamera)
        {
            moveToTargetLerp += Time.deltaTime / 0.5f;
            cameraTarget.transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, moveToTargetLerp));
        }
    }
    //Move the camera to a specific position within the selected time
    public void MoveToTarget(Vector3 selectedPosition, bool overrideTime = false, float time = 0)
    {
        MoveToTarget(selectedPosition, cameraTarget.transform.position, overrideTime, time);
    }

    public void MoveToTarget(Vector3 selectedPosition, Vector3 cameraStartPosition, bool overrideTime = false, float time = 0)
    {
        movingCamera = true;

        if (overrideTime == false)
        {
            moveToTargetLerp = time;
        }

        if (cameraStartPosition == null)
        {
            startPosition = cameraTarget.transform.position;
        }
        else
        {
            startPosition = cameraStartPosition;
        }

        targetPosition = selectedPosition;
    }

    public void SetCameraTime(float newTime)
    {
        moveToTargetLerp = newTime;
    }

    public Vector3 GetCameraPosition()
    {
        return cameraTarget.transform.position;
    }
}
