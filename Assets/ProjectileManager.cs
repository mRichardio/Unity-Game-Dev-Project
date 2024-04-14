using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ProjectileManager : MonoBehaviour
{
    GameManager gameManager;

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

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Projectile Collision" + "- Collider:" + collision.gameObject.name);

        if (collision.gameObject.name == "Enemy Basic" || collision.gameObject.name == "Enemy Light" || collision.gameObject.name == "Enemy Heavy")
        {
            //Debug.Log("Enemy Hit Collision" + "- Collider:" + collision.gameObject.name);
            // Damage the enemy
            collision.gameObject.GetComponent<Enemy>().TakeDamage(Damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.name == "Enemy Body")
        {
            Transform enemyComponent = collision.gameObject.transform.parent;
            Enemy script = enemyComponent.GetComponent<Enemy>();
            Debug.Log("Parent: " + enemyComponent.name);
            if (enemyComponent != null)
            {
                script.TakeDamage(Damage);
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.name == "CritPoint")
        {
            Enemy enemyComponent = collision.gameObject.transform.parent.GetComponent<Enemy>();
            Debug.Log("Parent: " + enemyComponent.name);
            if (enemyComponent != null)
            {
                Debug.Log("Crit Damage: " + Damage * CritMultiplier);
                enemyComponent.TakeDamage(Damage * CritMultiplier);
            }
            Destroy(gameObject);
        }
    }

    private GameObject GetWeapon()
    {
        GameObject player = GameObject.Find("Player");
        Transform weaponAttach = player.transform.Find("WeaponAttach");
        return weaponAttach.GetChild(0).gameObject;
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

    public void UpgradeDamage(int upgAmount)
    {
        Weapon = GetWeapon();

        float multiplier = BasicMultiplier; // Default multiplier

        if (Weapon != null)
        {
            if (Weapon.name == "WeaponBasic")
            {
                multiplier = BasicMultiplier;
            }
            else if (Weapon.name == "WeaponMedium")
            {
                multiplier = MediumMultiplier;
            }
            else if (Weapon.name == "WeaponEpic")
            {
                multiplier = EpicMultiplier;
            }
        }

        Damage += upgAmount * multiplier;
    }

    public void UpgradePower(int upgAmount)
    {
        Weapon = GetWeapon();

        float multiplier = BasicMultiplier; // Default multiplier

        if (Weapon != null)
        {
            if (Weapon.name == "WeaponBasic")
            {
                multiplier = BasicMultiplier;
            }
            else if (Weapon.name == "WeaponMedium")
            {
                multiplier = MediumMultiplier;
            }
            else if (Weapon.name == "WeaponEpic")
            {
                multiplier = EpicMultiplier;
            }
        }

        Power += upgAmount * multiplier;
    }
}
