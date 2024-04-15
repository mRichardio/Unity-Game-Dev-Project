using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    Rigidbody rb;

    // Projectile Stats
    public float Damage;
    public float Power;
    public float ExpandedRadius;

    // Destroy Interval
    public float DestroyInterval = 3.0f;
    private float nextDestroyTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        nextDestroyTime = Time.time + DestroyInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the projectile if it goes below the map
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }

        // Destroy the projectile after a certain amount of time
        if (Time.time >= nextDestroyTime)
        {
            Destroy(gameObject);
            nextDestroyTime = Time.time + DestroyInterval;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Expand radius of projectile collider when it hits something then check for enemies in the radius
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.radius = ExpandedRadius;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collider.radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.name.Contains("Enemy"))
            {
                Enemy enemyComponent = hitCollider.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.TakeDamage(Damage);
                }
            }
        }
    }
}
