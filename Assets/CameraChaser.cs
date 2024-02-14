using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChaser : MonoBehaviour
{
    public Transform target;
    public float speed = 5.0f;
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    public float followSpeed = 5f;

    // Update is called once per frame
    private void Update()
    {
        float h = speed * Input.GetAxis("Mouse X");
        transform.RotateAround(target.position, Vector3.up, h);
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
