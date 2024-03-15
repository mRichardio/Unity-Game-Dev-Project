using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int BaseHealth;

    private int currentHealth;

    void Start()
    {
        currentHealth = BaseHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            Time.timeScale = 0;
            Debug.Log("You Died!");
        }
    }

    public void UpgradeHealth(int upgAmount)
    {
        // Player Health
        BaseHealth += upgAmount;
    }
}
