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
    GameManager gameManager;
    ProjectileManager projectileManager;

    public GameObject ForwardMarker;

    // Player
    public float BaseMoney;
    public float CurrentMoney;
    public int BaseHealth;
    public int currentHealth;
    public float Damage; // Current Damage
    public float DefaultDamage;
    public float Power;// Current Power

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
    public float Sensitivity;
    private Vector2 PlayerMouseInput;
    private float xRot;
    public float MaxVelocity = 1.5f;

    // Weapons
    public GameObject BasicWeapon;
    public GameObject MediumWeapon;
    public GameObject EpicWeapon;
    public int weaponPrestige;
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

    // Weapon Damage Multipliers
    public float BasicMultiplier = 1;
    public float MediumMultiplier = 2;
    public float EpicMultiplier = 3;

    // Shop
    public GameObject HealthPriceText;
    public GameObject WeaponDMGPriceText;
    public GameObject WeaponPowerPriceText;
    public GameObject WeaponPrestigeText;

    public int MaxHealthUpgrade;
    public int MaxWeaponDMGUpgrade;
    public int MaxWeaponPowerUpgrade;

    public int CurrentHealthUpgrade;
    public int CurrentWeaponDMGUpgrade;
    public int CurrentWeaponPowerUpgrade;

    public int HealthUpgradeCost;
    public int WeaponDMGUpgradeCost;
    public int WeaponPowerUpgradeCost;
    public int PrestigeWeaponCost;

    public int PriceMultiplier;


    // Start is called before the first frame update
    void Start()
    {
        target = transform; // Need to look at
        rb = GetComponent<Rigidbody>();
        BuildCamera.SetActive(false);
        PlayerCamera.SetActive(true);
        weaponPrestige = 0;
        isEquipped = false;
        BaseHealth = 100;
        BaseMoney = 0;
        currentHealth = BaseHealth;
        CurrentMoney = BaseMoney;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        // Update Shop Prices
        UpdateShopPrices();

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

        // Screenshots
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                string fileName = "Screenshot-" + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + ".png";
                ScreenCapture.CaptureScreenshot(fileName);
                Debug.Log("Screenshot Taken");
            }
        }


        // Hold to enable mouse cursor when build mode isnt active
        if (Input.GetKey(KeyCode.LeftAlt) && BuildCamera.active == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Camera Rotation
        {

            PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            MovePlayerCamera();
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
            // sets cursor to be visible initially
            if (BuildCamera.active == true)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (gameManager.IsPaused == true)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // Disable camera movement when cursor is on screen
        {
            if (Cursor.visible == true && Cursor.lockState == CursorLockMode.None)
            {
                Sensitivity = 0;
            }
            else
            {
                Sensitivity = 4;
            }

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

    public void ToggleBuildMode()
    {
        if (Input.GetKeyDown(KeyCode.V) && gameManager.isPreparing && !gameManager.isPlaying)
        {
            if (BuildCamera.activeSelf)
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

        if (gameManager.isPlaying)
        {
            if (BuildCamera.activeSelf)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                BuildCamera.SetActive(false);
                PlayerCamera.SetActive(true);
            }
        }
    }

    public void PresitgeWeapon(int upgAmount)
    {
        Debug.Log(weaponPrestige);
        // Weapon Prestige
        if (CurrentMoney >= PrestigeWeaponCost && weaponPrestige < 2)
        {
            CurrentMoney -= PrestigeWeaponCost;
            PrestigeWeaponCost *= PriceMultiplier;
            weaponPrestige += upgAmount;
            DestroyWeapon();
            isEquipped = false;
        }
    }

    public void UpgradeDamage(int upgAmount)
    {
        GameObject player = GameObject.Find("Player");
        Transform weaponAttach = player.transform.Find("WeaponAttach");
        string weaponName = weaponAttach.GetChild(0).gameObject.name;

        float multiplier = BasicMultiplier; // Default multiplier

        if (weaponName == "WeaponBasic")
        {
            multiplier = BasicMultiplier;
        }
        else if (weaponName == "WeaponMedium")
        {
            multiplier = MediumMultiplier;
        }
        else if (weaponName == "WeaponEpic")
        {
            multiplier = EpicMultiplier;
        }


        if (CurrentMoney >= WeaponDMGUpgradeCost && CurrentWeaponDMGUpgrade < MaxWeaponDMGUpgrade)
        {
            CurrentMoney -= WeaponDMGUpgradeCost;
            WeaponDMGUpgradeCost *= PriceMultiplier;
            CurrentWeaponDMGUpgrade++;
            Damage += upgAmount * multiplier;

            UpdateShopPrices();
        }
    }

    public void UpgradePower(int upgAmount)
    {
        GameObject player = GameObject.Find("Player");
        Transform weaponAttach = player.transform.Find("WeaponAttach");
        string weaponName = weaponAttach.GetChild(0).gameObject.name;

        float multiplier = BasicMultiplier; // Default multiplier

        if (weaponName == "WeaponBasic")
        {
            multiplier = BasicMultiplier;
        }
        else if (weaponName == "WeaponMedium")
        {
            multiplier = MediumMultiplier;
        }
        else if (weaponName == "WeaponEpic")
        {
            multiplier = EpicMultiplier;
        }


        if (CurrentMoney >= WeaponPowerUpgradeCost && CurrentWeaponPowerUpgrade < MaxWeaponPowerUpgrade)
        {
            CurrentMoney -= WeaponPowerUpgradeCost;
            WeaponPowerUpgradeCost *= PriceMultiplier;
            CurrentWeaponPowerUpgrade++;
            Power += upgAmount * multiplier;

            UpdateShopPrices();
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
        Transform weaponParent = GameObject.Find("WeaponAttach").transform;

        if (isEquipped == false)
        {
            // Ensure that the weaponPrestige is within the correct range
            Debug.Assert(weaponPrestige == 0 || weaponPrestige == 1 || weaponPrestige == 2);

            // Basic Weapon
            if (weaponPrestige == 0)
            {
                GameObject createdWeapon = Instantiate(BasicWeapon, weaponParent.transform.position, Quaternion.LookRotation(ForwardMarker.transform.position - transform.position), weaponParent);
                createdWeapon.name = ("WeaponBasic");
                isEquipped = true;
            }

            // Medium Weapon
            if (weaponPrestige == 1)
            {
                GameObject createdWeapon = Instantiate(MediumWeapon, weaponParent.transform.position, Quaternion.LookRotation(ForwardMarker.transform.position - transform.position), weaponParent);
                createdWeapon.name = ("WeaponMedium");
                isEquipped = true;
            }

            // Epic Weapon
            if (weaponPrestige == 2)
            {
                GameObject createdWeapon = Instantiate(EpicWeapon, weaponParent.transform.position, Quaternion.LookRotation(ForwardMarker.transform.position - transform.position), weaponParent);
                createdWeapon.name = ("WeaponEpic");
                isEquipped = true;
            }
        }
    }

    public void DestroyWeapon()
    {
        GameObject Weapon = gameObject.transform.Find("WeaponAttach").transform.GetChild(0).gameObject;
        Destroy(Weapon);
    }

    private void MovePlayerCamera()
    {
        // Reference: https://www.youtube.com/watch?v=b1uoLBp2I1w Only used for camera rotation
        xRot = Mathf.Clamp(xRot, -15f, 40f);
        xRot -= PlayerMouseInput.y * Sensitivity;

        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);      
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
    public void UpdateShopPrices()
    {
        if (CurrentHealthUpgrade >= MaxHealthUpgrade) { HealthPriceText.GetComponent<TextMeshProUGUI>().text = "Maxed"; }
        else { HealthPriceText.GetComponent<TextMeshProUGUI>().text = HealthUpgradeCost.ToString(); }

        if (CurrentWeaponDMGUpgrade >= MaxWeaponDMGUpgrade) { WeaponDMGPriceText.GetComponent<TextMeshProUGUI>().text = "Maxed"; }
        else { WeaponDMGPriceText.GetComponent<TextMeshProUGUI>().text = WeaponDMGUpgradeCost.ToString(); }

        if (CurrentWeaponPowerUpgrade >= MaxWeaponPowerUpgrade) { WeaponPowerPriceText.GetComponent<TextMeshProUGUI>().text = "Maxed"; }
        else { WeaponPowerPriceText.GetComponent<TextMeshProUGUI>().text = WeaponPowerUpgradeCost.ToString(); }

        if (weaponPrestige >= 2) { WeaponPrestigeText.GetComponent<TextMeshProUGUI>().text = "Maxed"; }
        else { WeaponPrestigeText.GetComponent<TextMeshProUGUI>().text = PrestigeWeaponCost.ToString(); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Small Collectable" && !collision.gameObject.GetComponent<Collectable>().isCollected)
        {
            Debug.Log("Small Collectable");
            collision.gameObject.GetComponent<Collectable>().isCollected = true;
            CurrentMoney += 50;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.name == "Medium Collectable" && !collision.gameObject.GetComponent<Collectable>().isCollected)
        {
            Debug.Log("Medium Collectable");
            collision.gameObject.GetComponent<Collectable>().isCollected = true;
            CurrentMoney += 100;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.name == "Large Collectable" && !collision.gameObject.GetComponent<Collectable>().isCollected)
        {
            Debug.Log("Large Collectable");
            collision.gameObject.GetComponent<Collectable>().isCollected = true;
            CurrentMoney += 150;
            Destroy(collision.gameObject);
        }
    }

}
