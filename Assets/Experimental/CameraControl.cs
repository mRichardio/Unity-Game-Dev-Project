using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float RotationSpeed;

    private Transform target;

    void Start()
    {
        target = transform;
    }

    void Update()
    {
        // Camera Rotation
        {
            float h = Input.GetAxis("Mouse X");
            target.localRotation *= Quaternion.Euler(0, h, 0 * RotationSpeed);
        }
    }
}
