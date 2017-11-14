using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
public class Health : MonoBehaviour
{
    //[SerializeField] private Color healthColor;
    //[SerializeField] private Color HealthColorBackground;
    //[SerializeField] private Color healthColorEnemy;
    //[SerializeField] private Color HealthColorBackgroundEnemy;
    //private Image healthBarBackground;

    public Image[] healthBar;
    [SerializeField] private Slider healthSlider;
    public GameObject bar;
    public GameObject floatingDmg;
    public Transform damagePosition;
    public Transform barParent;

    private int currentUnitHealth;
    private int maxUnitHealth;
    public UnitConfig unitConfig;
    MapConfig mapConfig;

    void Start()
    {
        mapConfig = FindObjectOfType<MapConfig>();
        unitConfig = GetComponent<UnitConfig>();
        /*if (unitConfig.unitClassStats == null)
        {
            //unitConfig.unitClassStats = AssetDatabase.LoadAssetAtPath<ClassStatsObject>("Assets/Scriptable Object/StatsRookie.asset");
            Debug.LogWarning("Couldn't find Class, using default class");
        }*/
        InitiateUnitHealth();
    }

    private void Update()
    {
        
    }

    //set health values
    void InitiateUnitHealth()
    {
        healthBar = new Image[2];
        healthBar[1] = healthSlider.transform.GetChild(0).GetComponent<Image>();
        healthBar[0] = healthSlider.transform.GetChild(1).GetComponentInChildren<Image>();
        //Set health based on class
        currentUnitHealth = unitConfig.unitClassStats.maxUnitHealth;
        maxUnitHealth = unitConfig.unitClassStats.maxUnitHealth;

        //Update health slider
        healthSlider.maxValue = maxUnitHealth;
        healthSlider.value = currentUnitHealth;

        for(int i = 0; i < healthBar.Length; i++)
        {
            healthBar[i].color = unitConfig.unitColor[i];
        }

        for (int i = 0; i < currentUnitHealth; i++)
        {
            Instantiate(bar, barParent);
        }
        UpdateUnitHealth();
    }

    public void TakeDamage(int damageAmount, WeaponInfoObject weapon)
    {
        GameObject dmg = Instantiate(floatingDmg, damagePosition.position, Quaternion.Euler(transform.GetChild(0).localEulerAngles));
        Text[] dmgText = dmg.GetComponentsInChildren<Text>();
        //Check if miss
        CalculationManager.HitCheck(weapon, mapConfig.turnSystem.distance);
        if (CalculationManager.hit == false)
        {
            dmgText[0].text = "Missed!";
            dmgText[1].text = "-";
            //Temporary lazy code preventing zombies from missing
            CalculationManager.hit = true;
            Debug.Log("miss");
        }
        else
        {
            dmgText[1].text = CalculationManager.damage.ToString();
            currentUnitHealth -= CalculationManager.damage;
            if (currentUnitHealth <= 0)
            {
                if (!unitConfig.isFriendly)
                {
                    unitConfig.SetUnitState(UnitConfig.UnitState.Dead);
                    mapConfig.turnSystem.enemyUnits.Remove(unitConfig);
                    mapConfig.turnSystem.killCount++;
                }
                else
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

    public void KillUnit()
    {
        //Remove gameobject from playerUnits List in TurnSystem
        if (unitConfig.mapConfig == null)
            unitConfig.mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        unitConfig.mapConfig.tileMap.removeUnitMapData(unitConfig.tileX, unitConfig.tileY);
        if(unitConfig.isFriendly)
            unitConfig.mapConfig.turnSystem.playerUnits.Remove(unitConfig);
        else
        {
            //unitConfig.mapConfig.turnSystem.enemyUnits.Remove(unitConfig);
        }

        Destroy(gameObject);
    }
    public void RestoreHealthFull()
    {
        currentUnitHealth = maxUnitHealth;
        UpdateUnitHealth();
    }
    public void AddHealth(int healthRestored)
    {
        currentUnitHealth += healthRestored;
        if (currentUnitHealth > maxUnitHealth)
        {
            currentUnitHealth = maxUnitHealth;
        }
        UpdateUnitHealth();
    }
    public void SubractHealth(int healthRemoved)
    {
        currentUnitHealth -= healthRemoved;
        UpdateUnitHealth();
        if (currentUnitHealth <= 0)
        {
            currentUnitHealth = 0;
            KillUnit();
        }
    }
    public void Healing(int healAmount, WeaponInfoObject weapon)
    {
        GameObject heal = Instantiate(floatingDmg, damagePosition.position, Quaternion.Euler(transform.GetChild(0).localEulerAngles));
        Text[] healText = heal.GetComponentsInChildren<Text>();
        
        
        healText[1].text = CalculationManager.damage.ToString();
        currentUnitHealth += CalculationManager.damage;
            
        UpdateUnitHealth();
        
    }
}
