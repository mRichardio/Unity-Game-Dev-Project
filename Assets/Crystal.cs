using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Crystal : MonoBehaviour
{
    PlayerController playerController;

    public int BaseHealth;
    public int CurrentHealth;
    public int MaxHealthUpgrade;
    public int CurrentHealthUpgrade;
    public int HealthUpgradeCost;
    public int PriceMultiplier;
    public bool isAlive;

    // Damage Interval
    public float DamageInterval = 3.0f;
    private float nextDamageTime = 0.0f;

    // Enemy damage
    public int DamagePerEnemy = 1000;

    // List of enemies currently inside the trigger
    private List<Collider> enemiesInTrigger = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        // Set the damage time
        nextDamageTime = Time.time + DamageInterval;

        isAlive = true;
    }

    private void Update()
    {
        DetectEnemies();

        if (Time.time >= nextDamageTime)
        {
            foreach (var enemy in enemiesInTrigger)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    TakeDamage(DamagePerEnemy);
                }
            }
            // Set the next damage time
            nextDamageTime = Time.time + DamageInterval;
        }
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
            isAlive = false;
            Debug.Log("Your crystal has Died!");
        }
    }

    void DetectEnemies()
    {
        Vector3 detectionCenter = transform.position;
        float detectionRadius = transform.localScale.y * 10;
        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, detectionRadius);
        List<Collider> currentEnemies = new List<Collider>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.name.Contains("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy <= detectionRadius)
                {
                    currentEnemies.Add(hitCollider);
                }
            }
        }

        // Update enemies in range
        enemiesInTrigger = currentEnemies;
    }

}
