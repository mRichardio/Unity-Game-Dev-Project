using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public Transform[] checkpoints; 
    public float speed = 5f; 
    private int currentCheckpointIndex = 0; 
    private bool movingForward = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsCheckpoint();
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
            transform.position += movementDirection * speed * Time.deltaTime;
        }


    }

}
