using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {
    //Time
    [SerializeField]
    private float countdown;
    private void Start()
    {
        DestroyObject(gameObject, countdown);
    }
    private void Update()
    {
        
    }
}
