using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public GameObject BasicWeapon;
    public GameObject MediumWeapon;
    public GameObject EpicWeapon;
    public Transform weaponParent;
    public GameObject ForwardMarker;
    public Button shopBtn;

    GameObject BuildCamerParent;
    GameObject BuildCamera;

    private int weaponPrestige;
    private bool isEquipped;

    // Projectile Related
    public GameObject Projectile;
    public GameObject FiringPoint;
    Transform ProjectileParent;

    // Weapon Stats
    public float Damage;
    public float Power;

    void Start()
    {
        weaponPrestige = 0;
        isEquipped = false;

        BuildCamerParent = GameObject.Find("BuildCameraParent");
        BuildCamera = BuildCamerParent.transform.Find("BuildCamera").gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon();
        }


        if (Input.GetKey(KeyCode.Mouse0) && (BuildCamera == null || !BuildCamera.activeSelf))
        {
            GameObject createdProjectile = Instantiate(Projectile, FiringPoint.transform.position, Quaternion.identity, ProjectileParent);
            createdProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * Power);
        }
    }

    public void PresitgeWeapon(int upgAmount)
    {
        Debug.Log(weaponPrestige);
        // Weapon Prestige
        if (weaponPrestige >= 2) // This has to be 2 as the weaponPrestige is 0, 1, 2
        {
            shopBtn.interactable = false;
            shopBtn.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Max Prestige";
        }
        else
        {
            weaponPrestige += upgAmount;
            Destroy(GameObject.Find("Weapon"));
            isEquipped = false;
        }
    }

    public void EquipWeapon()
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
