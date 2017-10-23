using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {
    //Time
    [SerializeField]
    private float countdown;
    private void Start()
    {
        DestroyObject(this, countdown);
    }
    private void Update()
    {
        
    }
}
