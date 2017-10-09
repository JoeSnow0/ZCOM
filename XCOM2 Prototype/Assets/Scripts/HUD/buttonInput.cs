using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class buttonInput : MonoBehaviour {
    public KeyCode useAbility;
    public Button abilityButton;

    private void Start()
    {
        abilityButton = GetComponent<Button>();
    }


	void Update ()
    {
       if (Input.GetKeyDown(useAbility))
        {
            abilityButton.onClick.Invoke();
        }
	}
}
