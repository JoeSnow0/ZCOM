using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageElements : MonoBehaviour {
    public Image[] elements;
    [Range(0, 1)] public float transparencyMax;
    [Range(0, 1)] public float transparencyMin;

	void Start () {
        elements = GetComponentsInChildren<Image>();
	}

	void Update () {
		
	}
}
