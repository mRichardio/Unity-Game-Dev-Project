using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    // Base

    Transform target;
    Rigidbody rb;


    public GameObject ForwardMarker;

    // Camera
    public GameObject PlayerCamera;
    public GameObject BuildCamera;

    // Movement
    public float MovementSpeed;
    public float MaxSpeed;
    private bool isSprinting = false;

    // Advanced Movement
    public int JumpPower;
    public int RotationSpeed;
    public float MaxVelocity = 1.5f;

    // Weapons
    public GameObject Weapon;
    public bool isEquipped;

    // Start is called before the first frame update
    void Start()
    {
        target = transform; // Need to look at
        rb = GetComponent<Rigidbody>();
        BuildCamera.SetActive(false);
        PlayerCamera.SetActive(true);
        isEquipped = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Camera Rotation

        {
            float h = Input.GetAxis("Mouse X");
            target.localRotation *= Quaternion.Euler(0, h, 0 * RotationSpeed);
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
                if (BuildCamera.activeSelf == true)
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

        // Weapon Handling

        {
            // Equip Weapon
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Transform weaponParent = GameObject.Find("WeaponAttatch").transform;

                if (isEquipped == false)
                {
                    GameObject createdWeapon = Instantiate(Weapon, weaponParent.transform.position, Quaternion.LookRotation(ForwardMarker.transform.position - transform.position), weaponParent);
                    // 
                    createdWeapon.name = ("Weapon");
                    isEquipped = true;
                }
                else
                {
                    Destroy(GameObject.Find("Weapon"));
                    isEquipped = false;
                }
            }

            // Fire Weapon
            if (Input.GetMouseButtonDown(0))
            {
                if (Weapon != null)
                {
                    
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
