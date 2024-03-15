using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed;
    public float MaxSpeed;
    public int JumpPower;

    public Rigidbody rb;
    public bool isSprinting { get; set; }
    public bool isJumping { get; set; }

    GameObject BuildCamerParent;
    GameObject BuildCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        BuildCamerParent = GameObject.Find("BuildCameraParent");
        BuildCamera = BuildCamerParent.transform.Find("BuildCamera").gameObject;
    }

    void FixedUpdate()
    {
        // Basic Movement
        {
            // Move Forward/Back
            if (Input.GetKey(KeyCode.W) && BuildCamera.active == false)
            {
                rb.velocity += transform.forward * MovementSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S) && BuildCamera.active == false)
            {
                rb.velocity += -transform.forward * MovementSpeed * Time.deltaTime;
            }

            // Move Left/Right
            if (Input.GetKey(KeyCode.A) && BuildCamera.active == false)
            {

                rb.velocity += -transform.right * MovementSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) && BuildCamera.active == false)
            {
                rb.velocity += transform.right * MovementSpeed * Time.deltaTime;
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && BuildCamera.active == false && isJumping == false)
            {
                rb.velocity += transform.up * JumpPower;
            }

            // Sprint
            if (Input.GetKey(KeyCode.LeftShift) && BuildCamera.active == false)
            {
                isSprinting = true;
                MovementSpeed = 50;
                MaxSpeed = 20;
            }
        }

        // Advanced Movement
        {
            // Resets Back to Walking Speed
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = false;
                ResetSpeed();
            }

            // Clamp Velocity
            // REFERENCE: https://www.youtube.com/watch?v=7NMsVub5NZM
            if (rb.velocity.magnitude > MaxSpeed)
            {
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
            }
        }
    }

    void ResetSpeed()
    {
        MovementSpeed = 20;
        MaxSpeed = 10;
    }

    public void UpgradeSpeed(int upgAmount)
    {
        // Player Speed
        MovementSpeed += upgAmount;
    }
}
