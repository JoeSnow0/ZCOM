using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accuracy : MonoBehaviour
{

    int hitRoll;
    int hitChance;

    public void Hit()
    {
        //hitRoll;
        //hitRoll = GetComponent<RandomRange>().RollDice(1, 100);
        if (hitRoll <= hitChance)
        {
            //Run damage script
        }
        else
        {
            //Return "Missed" Message
        }

    }

}
