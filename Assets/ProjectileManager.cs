using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ProjectileManager : MonoBehaviour
{
    GameManager gameManager;
    PlayerController playerController;

    // Weapon
    private GameObject Weapon;

    // Projectile Stats
    public float Damage; // Current Damage
    public float DefaultDamage;
    public float Power;// Current Power
    public float DefaultPower;
    public float CritMultiplier = 3;    

    // Weapon Damage Multipliers
    public float BasicMultiplier = 1;
    public float MediumMultiplier = 2;
    public float EpicMultiplier = 3;

    // Destroy Interval
    public float DestroyInterval = 3.0f;
    private float nextDestroyTime = 0.0f;

    // Error Message

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Get parent Weapon
        GameObject player = GameObject.Find("Player");
        Transform weaponAttach = player.transform.Find("WeaponAttach");
        Weapon = weaponAttach.GetChild(0).gameObject;

        // Set the destroy time
        nextDestroyTime = Time.time + DestroyInterval;
    }

    // Update is called once per frame
    void Update()
    {
        {
            // Destroy the projectile if it goes below the map
            if (transform.position.y < 0)
            {
                Destroy(gameObject);
            }

            // Destroy the projectile after a certain amount of time
            if (Time.time >= nextDestroyTime)
            {
                Destroy(gameObject);
                nextDestroyTime = Time.time + DestroyInterval;
            }
        }

        // Update Damage and Power
        if (Weapon != null)
        {
            if (Weapon.name == "WeaponBasic")
            {
                Damage = playerController.Damage * BasicMultiplier;
                Power = playerController.Power * BasicMultiplier;
            }
            else if (Weapon.name == "WeaponMedium")
            {
                Damage = playerController.Damage * MediumMultiplier;
                Power = playerController.Power * MediumMultiplier;
            }
            else if (Weapon.name == "WeaponEpic")
            {
                Damage = playerController.Damage * EpicMultiplier;
                Power = playerController.Power * EpicMultiplier;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Enemy Basic" || collision.gameObject.name == "Enemy Light" || collision.gameObject.name == "Enemy Heavy")
        {
            // Damage the enemy
            collision.gameObject.GetComponent<Enemy>().TakeDamage(Damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.name == "Enemy Body")
        {
            Transform enemyComponent = collision.gameObject.transform.parent;
            Enemy script = enemyComponent.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                script.TakeDamage(Damage);
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.name == "CritPoint")
        {
            Enemy enemyComponent = collision.gameObject.transform.parent.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(Damage * CritMultiplier);
            }
            Destroy(gameObject);
        }
    }

    public void ResetProjectileValues()
    {
        GameObject player = GameObject.Find("Player");
        if (player.GetComponent<PlayerController>().weaponPrestige == 0)
        {
            Damage = DefaultDamage;
            Power = DefaultPower;
        }
    }
}
