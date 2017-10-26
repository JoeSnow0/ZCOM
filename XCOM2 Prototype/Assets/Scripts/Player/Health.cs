using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    // Use this for initialization
    public Color[] color;
    public Image[] healthBar;
    public Slider healthSlider;
    private int currentUnitHealth;
    private int maxUnitHealth;
    public GameObject floatingDmg;
    public Transform dmgStartPos;
    [HideInInspector]public UnitConfig unitConfig;
    public ClassStatsObject unitClassStats;


    void Start()
    {
        unitConfig = GetComponent<UnitConfig>();
        InitiateUnitHealth();
    }

    private void Update()
    {

    }

    //set health values
    void InitiateUnitHealth()
    {
        //Set health based on class
        currentUnitHealth = unitClassStats.maxUnitHealth;
        maxUnitHealth = unitClassStats.maxUnitHealth;
        //Update health slider
        healthSlider.maxValue = maxUnitHealth;
        healthSlider.value = currentUnitHealth;

        if (unitConfig.isFriendly)
        {
            //Set health bar Background Color for player
            healthBar[0].color = color[3];
            //Set set health bar color for player
            healthBar[1].color = color[2];
        }
        else
        {
            //Set health bar Background Color for enemy
            healthBar[0].color = color[1];
            //Set set health bar color for enemy
            healthBar[1].color = color[0];
        }
        UpdateUnitHealth();
    }

    public void TakeDamage(int damageAmount)
    {
        GameObject dmg = Instantiate(floatingDmg, dmgStartPos.position, Quaternion.Euler(transform.GetChild(0).localEulerAngles));
        //Check if miss
        if (CalculationManager.hit == false)
        {
            dmg.GetComponentInChildren<Text>().text = "Missed!";

            //Temporary lazy code preventing zombies from missing
            CalculationManager.hit = true;
        }

        else
        {
            dmg.GetComponentInChildren<Text>().text = "-" + damageAmount;
            currentUnitHealth -= damageAmount;
            healthSlider.value = currentUnitHealth;
            UpdateUnitHealth();
            if (currentUnitHealth <= 0)
            {
                //Remove gameobject from playerUnits List in TurnSystem
                //GameObject   unitConfig.map.GetComponent<TurnSystem>().playerUnits.Remove();
                //Destroy(gameObject);
            }

        }
    }
    
// Update the current health value of a unit
void UpdateUnitHealth()
    {
        //Sets color of healthbar based on health remaining
        //????????????????????????????????????????????????????????????
            
        
        
    }
}
