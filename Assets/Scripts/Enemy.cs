using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameManager gameManager;
    public float maxHealth = 100;
    public float currentHealth;

    public float Damage = 10.0f;
    public float MovementSpeed = 5f;
    public float ShrinkSpeed = .3f;

    // Collectables
    public Transform CollectableParent;
    public GameObject SmallCollectablePrefab;
    public GameObject MediumCollectablePrefab;
    public GameObject LargeCollectablePrefab;

    // Checkpoint System
    public Transform[] checkpoints;
    private int currentCheckpointIndex = 0;
    private bool movingForward = true;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CollectableParent = GameObject.Find("Collectables").transform;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Check if the GameManager is assigned
        if (gameManager == null)
        {
            Debug.LogError("GameManager is not assigned.");
            return;
        }
        else
        {
            Debug.Log("Enemy list: " + gameManager.Enemies.Count);
        }

    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsCheckpoint();
        CheckHealth();
    }

    // Sets the checkpoints for the enemy to follow
    public void SetEnemyCheckpoints(string checkpointName)
    {
        GameObject checkpointParent = GameObject.Find("Checkpoints");
        // Find the child called Checkpoint_A
        Transform Checkpoint = checkpointParent.transform.Find(checkpointName);
        // Get all the children of the Checkpoint_A and use each of their transforms as checkpoints
        checkpoints = new Transform[Checkpoint.childCount];
        for (int i = 0; i < Checkpoint.childCount; i++)
        {
            checkpoints[i] = Checkpoint.GetChild(i).transform;
        }
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
            MovementSpeed = 0;
            gameManager.Enemies.Remove(gameObject); // Removes the enemy from the enemies list

            transform.localScale -= Vector3.one * Time.deltaTime * ShrinkSpeed;

            // Shrink the enemy
            if (transform.localScale.x <= 0.01f)
            {
                if(gameObject.name == "Enemy Light") { GameObject c = Instantiate(SmallCollectablePrefab, transform.position, Quaternion.identity, CollectableParent); c.name = "Small Collectable"; }
                if(gameObject.name == "Enemy Basic") { GameObject c = Instantiate(MediumCollectablePrefab, transform.position, Quaternion.identity, CollectableParent); c.name = "Medium Collectable"; }
                if(gameObject.name == "Enemy Heavy") { GameObject c = Instantiate(LargeCollectablePrefab, transform.position, Quaternion.identity, CollectableParent); c.name = "Large Collectable"; }
                if(gameObject.name == "Enemy Boss") { gameManager.BossDefeated = true; }

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

        // Can be used to loop enemies back through the checkpoints if needed
        //if (currentCheckpointIndex >= checkpoints.Length)
        //{
        //    transform.position = checkpoints[0].position;
        //    currentCheckpointIndex = 0;
        //}
    }
}
