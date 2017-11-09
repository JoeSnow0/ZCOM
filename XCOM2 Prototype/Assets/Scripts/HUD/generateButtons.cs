using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class generateButtons : MonoBehaviour {

    AbilityInfoObject characterClass;
    [SerializeField]
    AbilityButton abilityButtonPrefab;
    abilityFunctions abilityFunctions;

    public void GenerateCurrentButtons(AbilityInfoObject characterClass)
    {

        foreach(AbilityInfo ability in characterClass.abilities)
        {
            //Create gameobject with veriables from Class/Abilities
            AbilityButton newButton = GameObject.Instantiate(abilityButtonPrefab, transform);
            newButton.abilityName.text = ability.name;
            newButton.abilityButton.GetComponent<buttonInput>().useAbility = ability.keybind;
            newButton.abilityIcon.sprite = ability.icon;
            newButton.abilityKeybind = ability.keybind;
            newButton.abilityButton.onClick.AddListener(() => { ability.callbackFunction.Invoke(); });
        }
    }
    public void ClearCurrentButtons()
    {
        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
