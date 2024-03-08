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
    public GameObject FiringPoint;
    Transform ProjectileParent;

    // Weapon Stats
    public float Damage;
    public float Power;

    // Start is called before the first frame update
    void Start()
    {
        // Find the BuildCamera even if it's inactive
        BuildCamerParent = GameObject.Find("BuildCameraParent");
        BuildCamera = BuildCamerParent.transform.Find("BuildCamera").gameObject;

        ProjectileParent = GameObject.Find("Projectiles").transform;
        FiringPoint = gameObject.transform.GetChild(0).gameObject;
        Debug.Log(BuildCamera);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && (BuildCamera == null || !BuildCamera.activeSelf))
        {
            GameObject createdProjectile = Instantiate(Projectile, FiringPoint.transform.position, Quaternion.identity, ProjectileParent);
            createdProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * Power);
        }
    }
}
