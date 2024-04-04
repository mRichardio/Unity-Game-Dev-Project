using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : MonoBehaviour
{

    // Build Camera Reference to check if it is active or not
    GameObject BuildCamerParent;
    GameObject BuildCamera;

    // Projectile Related
    public ProjectileManager ProjectileManager;
    public GameObject Projectile;
    private GameObject FiringPoint;
    Transform ProjectileParent;

    // Weapon Speed Multipliers
    public float BasicMultiplier = 1;
    public float MediumMultiplier = 2;
    public float EpicMultiplier = 3;

    // Fire Interval
    public float FiringInterval = .5f;
    private float nextFireTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Find the BuildCamera even if it's inactive
        BuildCamerParent = GameObject.Find("BuildCameraParent");
        BuildCamera = BuildCamerParent.transform.Find("BuildCamera").gameObject;

        ProjectileParent = GameObject.Find("Projectiles").transform;
        FiringPoint = gameObject.transform.GetChild(0).gameObject;

        ProjectileManager.ResetProjectileValues();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.identity; // Doesnt fire when holding alt
            if (Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftAlt) && (BuildCamera == null || !BuildCamera.activeSelf))
            {
            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + FiringInterval;
            }
        }
    }

    void FireProjectile()
    {
        GameObject player = GameObject.Find("Player");

        // Get the main camera
        Camera mainCamera = player.GetComponentInChildren<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        // Cast a ray from the center of the screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject projectileObject = Instantiate(Projectile, transform.position, Quaternion.identity, ProjectileParent);
            Vector3 direction = (hit.point - transform.position).normalized; // Gets the direction from the player to the hit point

            // Velocity
            Rigidbody projectileRigidbody = projectileObject.GetComponent<Rigidbody>();
            projectileRigidbody.velocity = direction * ProjectileManager.Power;
        }
        else
        {
            // Fires straight ahead if no hit
            GameObject projectileObject = Instantiate(Projectile, transform.position, Quaternion.identity, ProjectileParent);
            Rigidbody projectileRigidbody = projectileObject.GetComponent<Rigidbody>();
            projectileRigidbody.velocity = transform.forward * ProjectileManager.Power;
        }
    }

}
