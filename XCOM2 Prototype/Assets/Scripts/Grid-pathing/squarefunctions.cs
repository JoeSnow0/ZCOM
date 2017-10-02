using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squarefunctions : MonoBehaviour {
    float t = 0;

    Renderer rend;
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
    private void OnMouseOver()//need to optimise
    {
        currentColor = highlightedColor;
        rend.material.color = currentColor;
    }
    private void OnMouseExit()//need to optimise
    {
        currentColor = orgColor;
        rend.material.color = currentColor;
    }
}
