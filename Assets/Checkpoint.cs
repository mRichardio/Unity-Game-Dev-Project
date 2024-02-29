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
        if (checkpoints.Length == 0) return; // Exit if no checkpoints

        Transform targetCheckpoint = checkpoints[currentCheckpointIndex];
        // Use Vector3.Distance to find the distance to the checkpoint
        float distanceToCheckpoint = Vector3.Distance(transform.position, targetCheckpoint.position);
        if (distanceToCheckpoint < 0.1f) // Check if we've reached the checkpoint
        {
            // Logic to move to the next checkpoint or change direction
            if (movingForward)
            {
                if (currentCheckpointIndex < checkpoints.Length - 1)
                {
                    currentCheckpointIndex++;
                }
                else
                {
                    movingForward = false; // Change direction if you want to move back
                                           // Uncomment below if you want to loop
                                           // currentCheckpointIndex = 0;
                }
            }
            else
            {
                if (currentCheckpointIndex > 0)
                {
                    currentCheckpointIndex--;
                }
                else
                {
                    movingForward = true; // Change direction if you want to move forward again
                                          // Uncomment below if you want to loop
                                          // currentCheckpointIndex = checkpoints.Length - 1;
                }
            }
        }
        else
        {
            // Move towards the checkpoint
            Vector3 movementDirection = (targetCheckpoint.position - transform.position).normalized;
            transform.position += movementDirection * speed * Time.deltaTime;
        }
    }

}
