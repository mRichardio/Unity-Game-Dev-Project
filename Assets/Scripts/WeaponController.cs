using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : MonoBehaviour
{

    // Build Camera Reference to check if it is active or not
    GameObject BuildCamerParent;
    GameObject BuildCamera;

    // Projectile Related
    public GameObject Projectile;
    private GameObject FiringPoint;
    Transform ProjectileParent;

    // Weapon Stats
    public float Power;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && (BuildCamera == null || !BuildCamera.activeSelf))
        {
            if (Time.time >= nextFireTime)
            {
                //GameObject createdProjectile = Instantiate(Projectile, FiringPoint.transform.position, Quaternion.identity, ProjectileParent);
                FireProjectile();
                nextFireTime = Time.time + FiringInterval;
            }
        }
    }

    void FireProjectile()
    {
        // Instantiate projectile
        GameObject projectileObject = Instantiate(Projectile, transform.position, Quaternion.identity, ProjectileParent);

        // Calculate direction based on player's forward vector
        Vector3 playerForward = transform.forward;

        // Set the projectile's velocity in the direction of the player's forward vector
        Rigidbody projectileRigidbody = projectileObject.GetComponent<Rigidbody>();
        projectileRigidbody.velocity = playerForward * Power;
    }
}
