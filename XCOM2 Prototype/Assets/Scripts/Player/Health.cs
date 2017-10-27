using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class Health : MonoBehaviour
{
    [SerializeField]private Color[] color;
    [SerializeField]private Image[] healthBar;
    [SerializeField]private Slider healthSlider;
    [SerializeField] private ClassStatsObject unitClassStats;
    private int currentUnitHealth;
    private int maxUnitHealth;
    public GameObject floatingDmg;
    public Transform dmgStartPos;
    private UnitConfig unitConfig;
    


    void Start()
    {
        unitConfig = GetComponent<UnitConfig>();
        if (unitClassStats == null)
        {
            unitClassStats = AssetDatabase.LoadAssetAtPath<ClassStatsObject>("Assets/Scriptable Object/StatsRookie.asset");
            Debug.LogWarning("Couldn't find Class, using default class");
        }
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

        for (int i = 0; i < currentUnitHealth; i++)
        {
            Instantiate(barPrefab, barParent);
        }

        UpdateUnitHealth();
    }

    public void TakeDamage(int damageAmount)
    {
        GameObject dmg = Instantiate(floatingDmg, dmgStartPos.position, Quaternion.Euler(transform.GetChild(0).localEulerAngles));
        Text[] dmgText = dmg.GetComponentsInChildren<Text>();
        //Check if miss
        if (CalculationManager.hit == false)
        {
            dmgText[0].text = "Missed!";
            dmgText[1].text = "0";
            //Temporary lazy code preventing zombies from missing
            CalculationManager.hit = true;
        }
        else
        {
            dmgText[1].text = damageAmount.ToString();
            currentUnitHealth -= damageAmount;
            
            if (currentUnitHealth <= 0)
            {
                KillUnit();
            }
            UpdateUnitHealth();

        }
    }

    // Update the current health value of a unit
    void UpdateUnitHealth()
    {
        healthSlider.value = currentUnitHealth;
    }

    void KillUnit()
    {
        //Remove gameobject from playerUnits List in TurnSystem
        unitConfig.mapConfig.turnSystem.playerUnits.Remove(unitConfig);
        DestroyObject(this, 1);
    }
}
