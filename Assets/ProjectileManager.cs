using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    // Weapon
    GameObject Weapon;

    // Projectile Stats
    public int Damage = 10;
    public int Speed = 1000;

    // Weapon Multipliers
    public int BasicMultiplier = 1;
    public int MediumMultiplier = 2;
    public int EpicMultiplier = 3;

    // Destroy Interval
    public float DestroyInterval = 3.0f;
    private float nextDestroyTime = 0.0f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Get parent // TOO TIRED TO DO THS RN, MAKE THIS GET THE NAME OF THE WEAPON THAT IT IS FIRING FROM :) ATM IT IS JUST GETTING THE PARENT OF THE PROJECTILES
        Weapon = transform.parent.gameObject;
        Debug.Log("Weapon: " + Weapon.name);

        // Set the destroy time
        nextDestroyTime = Time.time + DestroyInterval;
        
        {
            // Add force to the projectile
            FirepProjectile(Speed);
        }
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
        if (collision.gameObject.name == "Enemy")
        {
            Debug.Log("Enemy Hit Collision" + "- Collider:" + collision.gameObject.name);
            // Damage the enemy
            collision.gameObject.GetComponent<Enemy>().TakeDamage(Damage);
            Destroy(gameObject);
        }
    }

    public void UpgradeDamage(int upgAmount)
    {
        // Weapon Damage
        Damage += upgAmount;
    }

    public void UpgradePower(int upgAmount)
    {
        // Weapon Power
        Speed += upgAmount;
    }

    public void FirepProjectile(int speed)
    {
        rb.AddForce(transform.forward * speed * BasicMultiplier);
    }
}
