using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    public TileMap tileMap;
    public Color[] color;
    public Image[] healthBar;
    public Text healthText;
    public Slider healthSlider;
    public Text apText;
    public bool isFriendly;
    [SerializeField, Range(0, 100)]
    public int health;
    int healthMax;
    [SerializeField, Range(0, 100)]
    public int damage;

    public int actions = 2;
    Unit target;

    public Animator animAP;
    public Transform dmgStartPos;
    public GameObject floatingDmg;

    public TurnSystem turnSystem;
    public bool isSelected = false;

    public BaseUnit baseUnit;

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
        healthText.text = health + "/" + healthMax;
        baseUnit = GetComponent<BaseUnit>();
    }

    void Update()
    {
        if (isSelected && Input.GetMouseButtonDown(1))
        {
            baseUnit.MoveNextTile();
        }

        if (isSelected && actions > 0)
        {
            GetComponentInChildren<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponentInChildren<Renderer>().material.color = Color.white;
        }
        apText.text = "(" + actions + ")";

        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
    }

    public void TakeDamage(int damageAmount)
    {
        GameObject dmg = Instantiate(floatingDmg, dmgStartPos.position, Quaternion.Euler(transform.GetChild(0).localEulerAngles));
        dmg.GetComponentInChildren<Text>().text = "-" + damageAmount;
        health -= damageAmount;
        healthText.text = health + "/" + healthMax;
        healthSlider.value = health;

        if (health <= 0)
        {
            turnSystem.destroyUnit(this);
        }
    }
}
