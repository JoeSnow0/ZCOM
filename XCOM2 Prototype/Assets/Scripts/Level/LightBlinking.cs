using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlinking : MonoBehaviour {

    Light[] lights;
    float time;
    public float delay;
    int currentLight = 0;

	void Start () {
        time = Time.time;
        lights = GetComponentsInChildren<Light>(true);
	}
	
	void LateUpdate () {
        time += Time.deltaTime;
        if (currentLight < lights.Length && time >= delay)
        {
            lights[currentLight].gameObject.SetActive(true);
            time = 0;
            currentLight++;
        }
        else if(currentLight >= lights.Length)
        {
            Destroy(this);
        }
	}
}
