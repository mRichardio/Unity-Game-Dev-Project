using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    // ASSET REFERENCES:

    // Health Bar: https://opengameart.org/content/health-bar
    // Heart: https://opengameart.org/content/heart-1616

    // Cameras
    public Transform BuildCam;
    private GameObject buildCamParent;

    public float Health = 100f;
    public float Damage = 10.0f;
    public float ShrinkSpeed = .3f;
    private int upgradeCount = 0;

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

    // UI
    public Canvas towerCanvas;
    public TextMeshProUGUI UpgCount;

    // Start is called before the first frame update
    void Start()
    {
        Turret = transform.GetChild(1).gameObject;
        Target = GameObject.Find("Player");

        // Sets the build camera
        buildCamParent = GameObject.Find("BuildCameraParent");
        Camera _BuildCam = buildCamParent.GetComponentInChildren<Camera>();
        BuildCam = _BuildCam.transform;

        // UI Event Camera
        if (!BuildCam.transform.gameObject.active)
        {
            towerCanvas = GetComponentInChildren<Canvas>();
            GameObject playerCameraObj = GameObject.Find("PlayerCamera");
            Camera playerCamera = playerCameraObj.GetComponent<Camera>();
            towerCanvas.worldCamera = playerCamera;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Sets event camera if build camera is not active
        if (!BuildCam.transform.gameObject.active)
        {
            towerCanvas = GetComponentInChildren<Canvas>();
            GameObject playerCameraObj = GameObject.Find("PlayerCamera");
            Camera playerCamera = playerCameraObj.GetComponent<Camera>();
            towerCanvas.worldCamera = playerCamera;
        }

        // Enemy Detection Interval

        {
            // Sets an interval for detecting enemies, better for performance
            if (Time.time >= nextDetectionTime)
            {
                DetectEnemies();
                nextDetectionTime = Time.time + detectionInterval;
            }
        }

        // Destroy Tower

        {
            // Destroy the turret if its health is 0    
            if (Health <= 0)
            {
                transform.localScale -= Vector3.one * Time.deltaTime * ShrinkSpeed;

                if (transform.localScale.x <= 0.01f)
                {
                    Destroy(gameObject);
                }
            }
        }

        // Turret Aiming

        {
            if (Target != null)
            {
                Turret.transform.LookAt(Turret.transform.position + (Turret.transform.position - Target.transform.position));
            }
        }

        // Turret Firing

        {
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
    }

    void DetectEnemies()
    {
        // REFERENCE: https://forum.unity.com/threads/physics-overlapsphere.476277/
        Vector3 detectionCenter = transform.position;
        float detectionRadius = transform.localScale.y * 10;

        var isCurrentTargetInRange = false;

        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, detectionRadius);
        if (hitColliders.Length != 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.name == "Enemy")
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (distanceToEnemy <= detectionRadius)
                    {
                        if (Target == hitCollider.gameObject)
                        {
                            isCurrentTargetInRange = true;
                        }
                    }
                }
            }

            // If the current target is not in range then find a new target
            if (!isCurrentTargetInRange)
            {
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.name == "Enemy")
                    {
                        float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
                        if (distanceToEnemy <= detectionRadius)
                        {
                            Target = hitCollider.gameObject;
                            isCurrentTargetInRange = true;
                            break;
                        }
                    }
                }
            }
        }

        // If the current target is not in range then clear the target
        if (!isCurrentTargetInRange)
        {
            Target = null;
        }
    }



    void InstantiateLaser()
    {
        if (Target != null && Target.name == "Enemy")
        {
            Transform LaserParent = GameObject.Find("Projectiles").transform;
            GameObject createdLaser = Instantiate(LaserProjectile, Turret.transform.position, Quaternion.LookRotation(Target.transform.position - Turret.transform.position), LaserParent);
            createdLaser.name = ("Laser");
        }
    }

    public void UpgradeTower(int upgAmount)
    {
        Debug.Log("Herro I have been called");
        int maxHealth = 500;
        int maxDamage = 50;

        if (Health < maxHealth)
        {
            Health += upgAmount;
        }
        if (Damage < maxDamage) 
        {
            Damage += upgAmount;
        }
        if (upgradeCount < 4)
        {
            upgradeCount++;
        }

        UpgCount.text = $"{upgradeCount}/4";
    }

    // for debugging purposes
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.y * 10);
    }
}

