using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class Health : MonoBehaviour
{
    //[SerializeField] private Color healthColor;
    //[SerializeField] private Color HealthColorBackground;
    //[SerializeField] private Color healthColorEnemy;
    //[SerializeField] private Color HealthColorBackgroundEnemy;
    //private Image healthBarBackground;
    [Tooltip("First image: Fill. Second image: Background.")]
    private Image[] healthBar;
    [SerializeField] private Slider healthSlider;
    public GameObject bar;
    public GameObject floatingDmg;
    public Transform damagePosition;
    public Transform barParent;

    private int currentUnitHealth;
    private int maxUnitHealth;
    private UnitConfig unitConfig;

    void Start()
    {
        

        unitConfig = GetComponent<UnitConfig>();
        if (unitConfig.unitClassStats == null)
        {
            unitConfig.unitClassStats = AssetDatabase.LoadAssetAtPath<ClassStatsObject>("Assets/Scriptable Object/StatsRookie.asset");
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

    public void TakeDamage(int damageAmount)
    {
        GameObject dmg = Instantiate(floatingDmg, damagePosition.position, Quaternion.Euler(transform.GetChild(0).localEulerAngles));
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
        unitConfig.mapConfig.tileMap.removeUnitMapData(unitConfig.tileX, unitConfig.tileY);
        if(unitConfig.isFriendly)
        unitConfig.mapConfig.turnSystem.playerUnits.Remove(unitConfig);
        else
        {
            unitConfig.mapConfig.turnSystem.enemyUnits.Remove(unitConfig);
        }
        DestroyObject(gameObject, 1);
    }
}
