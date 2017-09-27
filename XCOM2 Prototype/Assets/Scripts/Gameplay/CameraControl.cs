using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    float speed = 50.0f;
    void Start ()
    {

    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(GetComponent<Transform>().position.x + speed *Time.deltaTime, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(GetComponent<Transform>().position.x - speed * Time.deltaTime, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z - speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z + speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = new Vector3(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y - speed * Time.deltaTime, GetComponent<Transform>().position.z);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position = new Vector3(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y + speed * Time.deltaTime, GetComponent<Transform>().position.z);
        }
    }
}
