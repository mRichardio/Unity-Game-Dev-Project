using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    PlayerController playerController;

    public int BaseHealth;
    public int CurrentHealth;
    public int MaxHealthUpgrade;
    public int CurrentHealthUpgrade;
    public int HealthUpgradeCost;
    public int PriceMultiplier;

    // Damage Interval
    public float DamageInterval = 3.0f;
    private float nextDamageTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set the damage time
        nextDamageTime = Time.time + DamageInterval;
    }

    public int GetHealth()
    {
        return CurrentHealth;
    }

    public void UpgradeHealth(int upgAmount)
    {
        // Player Health
        if (BaseHealth < 150 && MaxHealthUpgrade > CurrentHealthUpgrade && playerController.CurrentMoney >= HealthUpgradeCost)
        {
            playerController.CurrentMoney -= HealthUpgradeCost;
            HealthUpgradeCost *= PriceMultiplier;
            CurrentHealthUpgrade++;
            BaseHealth += upgAmount;
            CurrentHealth += upgAmount;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Destroy(gameObject);
            Time.timeScale = 0;
            Debug.Log("Your crystal has Died!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
         // <----
        if (collision.gameObject.name.Contains("Enemy"))
        {
            if (Time.time >= nextDamageTime)
            {
                TakeDamage(10);
                nextDamageTime = Time.time + DamageInterval;
            }
        }

        //SphereCollider collider = GetComponent<SphereCollider>();
        //collider.radius = 3.5f;

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, collider.radius);
        //foreach (var hitCollider in hitColliders)
        //{
        //    if (hitCollider.name.Contains("Enemy"))
        //    {
        //        Enemy enemyComponent = hitCollider.GetComponent<Enemy>();
        //        if (enemyComponent != null)
        //        {
        //            if (Time.time >= nextDamageTime)
        //            {
        //                enemyComponent.TakeDamage(10);
        //                nextDamageTime = Time.time + DamageInterval;
        //            }

        //        }
        //    }
        //}
    }
}
