using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject PlayerCamera;
    public float MovementSpeed;
    public float RotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && PlayerCamera.active == false)
        {
            transform.position += Vector3.forward * Time.deltaTime * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.S) && PlayerCamera.active == false)
        {
            transform.position += Vector3.back * Time.deltaTime * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.A) && PlayerCamera.active == false)
        {
            transform.position += Vector3.left * Time.deltaTime * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.D) && PlayerCamera.active == false)
        {
            transform.position += Vector3.right * Time.deltaTime * MovementSpeed;
        }
    }
}
