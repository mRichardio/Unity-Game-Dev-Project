using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    // Base
    Transform target;
    Rigidbody rb;
    public GameObject ForwardMarker;

    // Player
    private int playerPrestige; // Needs implementing <----------------
    public int BaseHealth;
    private int currentHealth;

    // Camera
    public GameObject PlayerCamera;
    public GameObject BuildCamera;

    // Movement
    public float MovementSpeed;
    public float MaxSpeed;
    private bool isSprinting = false;
    private bool isJumping = false;

    // Advanced Movement
    public int JumpPower;
    public int RotationSpeed;
    public float MaxVelocity = 1.5f;

    // Weapons
    public GameObject BasicWeapon;
    public GameObject MediumWeapon;
    public GameObject EpicWeapon;
    private int weaponPrestige; // Will need to multiply weapon stats by the weaponPrestige // IMPORTANT <----------------
    public bool isEquipped;
    public Button prestigeWeaponBtn;

    // Audio
    private string CurrentGroundType;
    public AudioMixerSnapshot StoneSnapshot;
    public AudioMixerSnapshot MudSnapshot;

    // Footstep Trail
    public GameObject FootstepPrefab;
    public Transform FootstepParent;
    private float lastFootstepTime;
    public float FootstepFrequency;

    // Start is called before the first frame update
    void Start()
    {
        target = transform; // Need to look at
        rb = GetComponent<Rigidbody>();
        BuildCamera.SetActive(false);
        PlayerCamera.SetActive(true);
        weaponPrestige = 0;
        isEquipped = false;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            CurrentGroundType = hit.collider.name;
            if (hit.distance > 1.1f)
            {
                isJumping = true;
            }
            else { isJumping = false; }
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Basic Footstep Audio
        if (rb.velocity.magnitude > 0.1f && isSprinting == false)
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
                if (isJumping == false)
                {
                    lastFootstepTime = Time.time;
                    GameObject createdFootstep = Instantiate(FootstepPrefab, transform.position + Vector3.down * 0.7f, Quaternion.identity, FootstepParent);
                    Footstep f = createdFootstep.GetComponent<Footstep>();
                    f.ShrinkSpeed = UnityEngine.Random.Range(0.02f, 0.1f); // Had to use "UnityEngine here as without it Random.Range doesnt work here for some reason..."

                }
            }
        }

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

            ClampVelocity();
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

        // Build Mode

        {
            ToggleBuildMode();
        }

        // Weapon Handling

        {
            HandleWeaponEquip();
        }

    }

    void ResetSpeed()
    {
        MovementSpeed = 20;
        MaxSpeed = 10;
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

    public void ToggleBuildMode()
    {
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

    public void UpgradeSpeed(int upgAmount)
    {
        // Player Speed
        MovementSpeed += upgAmount;
    }

    public void PresitgeWeapon(int upgAmount)
    {
        Debug.Log(weaponPrestige);
        // Weapon Prestige
        if (weaponPrestige >= 2) // This has to be 2 as the weaponPrestige is 0, 1, 2
        {
            prestigeWeaponBtn.interactable = false;
            prestigeWeaponBtn.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Max Prestige";
        }
        else
        {
            weaponPrestige += upgAmount;
            Destroy(GameObject.Find("Weapon"));
            isEquipped = false;
        }
    }

    public void ClampVelocity()
    {
        // Clamp Velocity
        // REFERENCE: https://www.youtube.com/watch?v=7NMsVub5NZM
        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
        }
    }
    
    public void HandleWeaponEquip()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Transform weaponParent = GameObject.Find("WeaponAttatch").transform;

            if (isEquipped == false)
            {
                // Ensure that the weaponPrestige is within the correct range
                Debug.Assert(weaponPrestige == 0 || weaponPrestige == 1 || weaponPrestige == 2);

                // Basic Weapon
                if (weaponPrestige == 0)
                {
                    GameObject createdWeapon = Instantiate(BasicWeapon, weaponParent.transform.position, Quaternion.LookRotation(ForwardMarker.transform.position - transform.position), weaponParent);
                    createdWeapon.name = ("Weapon");
                    isEquipped = true;
                }

                // Medium Weapon
                if (weaponPrestige == 1)
                {
                    GameObject createdWeapon = Instantiate(MediumWeapon, weaponParent.transform.position, Quaternion.LookRotation(ForwardMarker.transform.position - transform.position), weaponParent);
                    createdWeapon.name = ("Weapon");
                    isEquipped = true;
                }

                // Epic Weapon
                if (weaponPrestige == 2)
                {
                    GameObject createdWeapon = Instantiate(EpicWeapon, weaponParent.transform.position, Quaternion.LookRotation(ForwardMarker.transform.position - transform.position), weaponParent);
                    createdWeapon.name = ("Weapon");
                    isEquipped = true;
                }
            }
            else
            {
                Destroy(GameObject.Find("Weapon"));
                isEquipped = false;
            }
        }
    }

    public void PrestigePlayer(int upgAmount)
    {
        // Player Prestige
        playerPrestige += upgAmount;
    }
}
