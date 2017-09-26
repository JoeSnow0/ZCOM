using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squarefunctions : MonoBehaviour {
    float t = 0;
    public float timer =0;
    Renderer rend;
    public Color currentColor;
    public Color orgColor;
	// Use this for initialization
	void Start () {
        rend = GetComponentInChildren<Renderer>();
        
	}
	
	// Update is called once per frame
	void Update () {
        
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            rend.material.color = currentColor;
            currentColor = Color.Lerp(orgColor, currentColor, timer);
            
        }
        else if(timer < 0)
        {
            timer = 0;
        }

	}
}
