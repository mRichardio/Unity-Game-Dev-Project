using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootstepManager : MonoBehaviour
{
    private PlayerMovement playerMovement;

    public AudioMixerSnapshot StoneSnapshot;
    public AudioMixerSnapshot MudSnapshot;
    public GameObject FootstepPrefab;
    public Transform FootstepParent;
    public float FootstepFrequency;

    private string CurrentGroundType;
    private float lastFootstepTime;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void FixedUpdate()
    {
        // Detect current ground type
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                CurrentGroundType = hit.collider.name;
                if (hit.distance > 1.1f)
                {
                    playerMovement.isJumping = true;
                }
                else { playerMovement.isJumping = false; }
            }
        }
        // Play footstep audio and create footstep visuals
        {
            if (playerMovement.rb.velocity.magnitude > 0.1f && playerMovement.isSprinting == false)
            {
                // Sets the audio snapshot depending on the ground type
                if (CurrentGroundType == "Stone")
                {
                    StoneSnapshot.TransitionTo(1);
                }
                if (CurrentGroundType == "Mud")
                {
                    MudSnapshot.TransitionTo(1);
                }

                // Footstep Trail
                if (lastFootstepTime + FootstepFrequency <= Time.time)
                {
                    if (playerMovement.isJumping == false)
                    {
                        lastFootstepTime = Time.time;
                        GameObject createdFootstep = Instantiate(FootstepPrefab, transform.position + Vector3.down * 0.7f, Quaternion.identity, FootstepParent);
                        Footstep f = createdFootstep.GetComponent<Footstep>();
                        f.ShrinkSpeed = UnityEngine.Random.Range(0.02f, 0.1f); // Had to use "UnityEngine here as without it Random.Range doesnt work here for some reason..."

                    }
                }
            }
        }
    }
}
