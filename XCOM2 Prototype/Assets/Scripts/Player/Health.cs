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

    public void TakeDamage(WeaponInfoObject weapon)
    {
        GameObject dmg = Instantiate(floatingDmg, damagePosition.position, Quaternion.Euler(transform.GetChild(0).localEulerAngles));
        Text[] dmgText = dmg.GetComponentsInChildren<Text>();
        //Check if miss
        CalculationManager.HitCheck(weapon, UnitConfig.accuracy);
        if (CalculationManager.hit == false)
        {
            dmgText[0].text = "Missed!";
            dmgText[1].text = "-";
            //Temporary lazy code preventing zombies from missing
            CalculationManager.hit = true;
        }
        else
        {
            dmgText[1].text = CalculationManager.damage.ToString();
            currentUnitHealth -= CalculationManager.damage;
            Instantiate(unitConfig.unitClassStats.hitParticleSystem, transform.GetChild(0).position, transform.rotation);
            if (currentUnitHealth <= 0)
            {
                //unitConfig.isDead = true;
                if (!unitConfig.isFriendly)
                {
                    
                    mapConfig.turnSystem.enemyUnits.Remove(unitConfig);
                }
                else
                {
                    mapConfig.turnSystem.playerUnits.Remove(unitConfig);
                }
                unitConfig.SetUnitState(UnitConfig.UnitState.Dead);
                mapConfig.turnSystem.AddKillCount(unitConfig.unitClassStats);
            }
            UpdateUnitHealth();
        }
        if(!unitConfig.isFriendly)// FOR TUTORIAL
            TurnSystem.hasShot = true; 

        TurnSystem.selectedTarget = null;
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

            unitConfig.SetUnitState(UnitConfig.UnitState.Dead);
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

    public int GetHealth()
    {
        return currentUnitHealth;
    }
}
