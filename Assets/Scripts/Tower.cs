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

    // Start is called before the first frame update
    void Start()
    {
        Turret = transform.GetChild(1).gameObject;
        Target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * ShrinkSpeed;

            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
            }
        }

        Turret.transform.LookAt(Turret.transform.position + (Turret.transform.position - Target.transform.position));
        //Turret.transform.LookAt(Target.transform.position);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject createdLaser = Instantiate(LaserProjectile, Turret.transform.position, Quaternion.LookRotation(Target.transform.position - Turret.transform.position));
        }
    }
}

