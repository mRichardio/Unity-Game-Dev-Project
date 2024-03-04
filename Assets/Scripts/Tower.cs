//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UIElements;
//using static UnityEngine.GraphicsBuffer;

//public class Tower : MonoBehaviour
//{
//    public float Health = 100f;
//    public float Damage = 10.0f;
//    public float ShrinkSpeed = .3f;

//    GameObject Turret;
//    public GameObject LaserProjectile;
//    public Transform Target;
//    public float RotationSpeed;

//    // Start is called before the first frame update
//    void Start()
//    {
//        Turret = transform.GetChild(1).gameObject;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Health <= 0) 
//        {
//            transform.localScale -= Vector3.one * Time.deltaTime * ShrinkSpeed;

//            if (transform.localScale.x <= 0.01f)
//            {
//                Destroy(gameObject);
//            }
//        }

//        Turret.transform.LookAt(Target);

//        if (Input.GetKeyDown(KeyCode.Q))
//        {
//            GameObject createdLaser = Instantiate(LaserProjectile, transform.position, Quaternion.identity);
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    public float Health = 100f;
    public float Damage = 10.0f;
    public float ShrinkSpeed = .3f;

    public GameObject Turret;
    public GameObject LaserProjectile;
    GameObject Target;
    //public float RotationSpeed;

    // Enemy detection
    public float detectionInterval = 1.0f;
    private float nextDetectionTime = 0.0f;

    // Fire Rate
    public float FiringInterval = .5f;
    private float nextFireTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Turret = transform.GetChild(1).gameObject;
        Target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit hit;

        //Vector3 p1 = Turret.transform.position;
        //float distanceToObstacle = 0;

        //// Cast a sphere wrapping character controller 10 meters forward
        //// to see if it is about to hit anything.
        //if (Physics.SphereCast(p1, Turret.transform.localScale.y / 2, Turret.transform.forward, out hit, 10))
        //{
        //    distanceToObstacle = hit.distance;
        //    Debug.Log("Hit " + distanceToObstacle);
        //}

        // Sets an interval for detecting enemies, better for performance
        if (Time.time >= nextDetectionTime)
        {
            DetectEnemies();
            nextDetectionTime = Time.time + detectionInterval;
        }

        // Destroy the turret if its health is 0    
        if (Health <= 0)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * ShrinkSpeed;

            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
            }
        }

        if (Target != null)
        {
            Turret.transform.LookAt(Turret.transform.position + (Turret.transform.position - Target.transform.position));
        }

        //Turret.transform.LookAt(Target.transform.position);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject createdLaser = Instantiate(LaserProjectile, Turret.transform.position, Quaternion.LookRotation(Target.transform.position - Turret.transform.position));
        }

        if (Time.time >= nextFireTime)
        {
            InstantiateLaser();
            nextFireTime = Time.time + FiringInterval;
        }
    }

    void DetectEnemies()
    {
        // REFERENCE: https://forum.unity.com/threads/physics-overlapsphere.476277/
        Vector3 detectionCenter = transform.position;
        float detectionRadius = transform.localScale.y * 10;

        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, detectionRadius);
        if (hitColliders.Length != 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.name == "Enemy")
                {
                    //Debug.Log("Found the enemy!");
                    Target = hitCollider.gameObject;
                }
            }
        }
    }

    void InstantiateLaser()
    {
        if (Target != null && Target.name == "Enemy")
        {
            Debug.Log("Firing laser!");
            Transform LaserParent = GameObject.Find("Projectiles").transform;
            GameObject createdLaser = Instantiate(LaserProjectile, Turret.transform.position, Quaternion.LookRotation(Target.transform.position - Turret.transform.position), LaserParent);
            createdLaser.name = ("Laser");
        }
    }

    // for debugging purposes
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.y * 10);
    }
}

