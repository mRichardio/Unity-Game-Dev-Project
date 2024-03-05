using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    Transform target;
    Rigidbody rb;
    public GameObject PlayerCamera;
    public GameObject BuildCamera;

    public float MovementSpeed;
    public float MaxSpeed;
    private bool isSprinting = false;

    public int JumpPower;
    public int RotationSpeed;
    public float MaxVelocity = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        target = transform; // Need to look at
        rb = GetComponent<Rigidbody>();
        BuildCamera.SetActive(false);
        PlayerCamera.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        // Camera Rotation

        {
            float h = Input.GetAxis("Mouse X");
            target.localRotation *= Quaternion.Euler(0, h, 0 * RotationSpeed);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

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
            if (Input.GetKeyDown(KeyCode.Space) && BuildCamera.active == false)
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

        // Build Mode

        {
            // Toggle Build Mode
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (BuildCamera.active == true)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    BuildCamera.SetActive(false);
                    PlayerCamera.SetActive(true);
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    BuildCamera.SetActive(true);
                    PlayerCamera.SetActive(false);
                }
            }
        }

    }

    void ResetSpeed()
    {
        MovementSpeed = 20;
        MaxSpeed = 10;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Demo")
        {
            if (collision.gameObject.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rbNew = collision.gameObject.AddComponent<Rigidbody>();
            }
        }
    }
}