using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomRange// : MonoBehaviour
{

    //int diceRolls;
    

    public static int RollDice(int numberOfDice, int numberOfSides)
    {
        int n = 0;
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
            n = n + Random.Range(1, numberOfSides + 1);
        }
        
        return(n);
	}
	
	
}
