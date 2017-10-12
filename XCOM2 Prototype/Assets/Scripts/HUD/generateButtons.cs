using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class generateButtons : MonoBehaviour {

    [SerializeField]
    AbilityInfoObject characterClass;
    [SerializeField]
    AbilityButton abilityButtonPrefab;
    

    private void Start()
    {

        foreach(AbilityInfo ability in characterClass.abilities)
        {
            // Create gameobject with info from ability/Class
            AbilityButton newButton = GameObject.Instantiate(abilityButtonPrefab, transform);
            newButton.abilityName.text = ability.name;
            newButton.abilityButton.GetComponent<buttonInput>().useAbility = ability.keybind;
            newButton.abilityIcon.sprite = ability.icon;
            newButton.abilityKeybind = ability.keybind;

            //newButton.abilityButton.onClick = ability.callbackFunction as Button.ButtonClickedEvent;
            //newButton.abilityButton.onClick.AddListener(ability.callbackFunction);
            //ability.callbackFunction
            newButton.abilityButton.onClick.AddListener(() => { ability.callbackFunction.Invoke(); });
        }
    }
    
}
