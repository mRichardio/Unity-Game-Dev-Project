using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float Health = 100f;
    public float ShrinkSpeed = .3f;

    // Start is called before the first frame update
    void Start()
    {
        
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
        
    }
}
