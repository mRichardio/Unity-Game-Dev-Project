using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;

    public float Damage = 10.0f;
    public float MovementSpeed = 5f;
    public float ShrinkSpeed = .3f;

    // Checkpoint System
    public Transform[] checkpoints;
    private int currentCheckpointIndex = 0;
    private bool movingForward = true;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsCheckpoint();
        CheckHealth();
    }

    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Laser")
        {
            TakeDamage(20); // Might wanna make a variable for this and set the damage in the laser script.

            MeshRenderer meshR = gameObject.GetComponent<MeshRenderer>();
            Material material = meshR.material;
            material.color = Color.white;
        }
    }

    void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * ShrinkSpeed;

            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    void MoveTowardsCheckpoint()
    {
        if (checkpoints.Length == 0 || currentCheckpointIndex >= checkpoints.Length) return; // Exit if no checkpoints or we've reached the last one

        Transform targetCheckpoint = checkpoints[currentCheckpointIndex];
        // Find the distance between two objects // https://forum.unity.com/threads/distance-between-two-objects.37918/
        float distanceToCheckpoint = Vector3.Distance(transform.position, targetCheckpoint.position);
        if (distanceToCheckpoint < 0.1f) // Checks if th checkpoint has vbeen reached
        {
            currentCheckpointIndex++; // Goes tto the next checkpoint in the array
        }
        else
        {
            // Then will move to the checkpoint
            Vector3 movementDirection = (targetCheckpoint.position - transform.position).normalized;
            transform.position += movementDirection * MovementSpeed * Time.deltaTime;
        }

        // I have this here for now because it looks cool
        if (currentCheckpointIndex >= checkpoints.Length)
        {
            transform.position = checkpoints[0].position;
            currentCheckpointIndex = 0;
        }
    }
}
