using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    //int diceRolls;
    int n;
    public int numberOfSides;
    public int numberOfDice;

    void Start ()
    {
        /*
        diceRolls = 0;
        while(true)
        {
            n = n + Random.Range(1, numberOfSides);
            diceRolls++;
            if (diceRolls == numberOfDice)
            {
                break;
            }
        }   */
        for (int i = 0; i < numberOfDice; i++)
        {
            n = n + Random.Range(1, numberOfSides);
        }
        
        print(n);
	}
	
	
}
