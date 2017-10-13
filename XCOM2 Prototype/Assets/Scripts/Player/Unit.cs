using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    public TileMap tileMap;
    public Color[] color;
    public Image[] healthBar;
    public Image[] actionPoints;
    //public Text healthText;
    public Slider healthSlider;
    public bool isFriendly;
    [SerializeField, Range(0, 10)]
    public int health;
    protected int healthMax;
    [SerializeField, Range(0, 10)]
    public int damage;

    public int actions = 2;
    Unit target;

    public Animator animAP;
    public Transform dmgStartPos;
    public GameObject floatingDmg;
    public GameObject bar;
    public Transform barParent;

    public TurnSystem turnSystem;
    public bool isSelected = false;

    public BaseUnit baseUnit;

    public WeaponInfoObject unitWeapon;

    void Start () {
        //Sets color of healthbar
        if (!isFriendly)
        {
            for(int i = 0; i <= 1; i++)
            {
                healthBar[i].color = color[i];
            }
        }
        healthMax = health;
        healthSlider.maxValue = healthMax;
        healthSlider.value = healthMax;
        baseUnit = GetComponent<BaseUnit>();
        turnSystem = GameObject.FindGameObjectWithTag("Map").GetComponent<TurnSystem>();
        

        for(int i = 0; i < healthMax; i++)
        {
            Instantiate(bar, barParent, false);
        }
    }

    void Update()
    {
        if(actions < 2)
        {
            actionPoints[0].color = color[2];
            if(actions < 1)
            {
                actionPoints[1].color = color[2];
            }
            else
            {
                actionPoints[1].color = color[3];
            }
        }
        else
        {
            actionPoints[0].color = color[3];
            actionPoints[1].color = color[3];
        }

        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
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
            health -= damageAmount;
            healthSlider.value = health;

            if (health <= 0)
            {
                turnSystem.destroyUnit(this);
            }
        }
    }
}
