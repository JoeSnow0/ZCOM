using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squarefunctions : MonoBehaviour {

    public Renderer rend;
    public Color currentColor;
    public Color orgColor;
    public Color highlightedColor;
	// Use this for initialization
	void Start () {
        rend = GetComponentInChildren<Renderer>();
        rend.material.color = currentColor;
    }
	
	// Update is called once per frame
	void Update () {
            
	}
}
