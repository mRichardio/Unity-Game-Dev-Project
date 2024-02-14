using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    Transform target;
    Rigidbody rb;
    public GameObject PlayerCamera;
    public GameObject BuildCamera;

    public float MovementSpeed;
    public int JumpPower;
    public int RotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        target = transform; // Need to look at
        rb = GetComponent<Rigidbody>();
        BuildCamera.SetActive(false);
        PlayerCamera.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        float h = Input.GetAxis("Mouse X");
        target.localRotation *= Quaternion.Euler(0, h, 0 * RotationSpeed);

        if (Input.GetKey(KeyCode.W) && BuildCamera.active == false)
        {
            rb.velocity += transform.forward * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) && BuildCamera.active == false)
        {
            rb.velocity += -transform.forward * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A) && BuildCamera.active == false)
        {
            rb.AddTorque(-transform.up * RotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) && BuildCamera.active == false)
        {
            rb.AddTorque(transform.up * RotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && BuildCamera.active == false)
        {
            rb.velocity += transform.up * JumpPower;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (BuildCamera.active == true)
            {
                BuildCamera.SetActive(false);
                PlayerCamera.SetActive(true);
            }
            else
            {
                BuildCamera.SetActive(true);
                PlayerCamera.SetActive(false);
            }
        }
    }
}
