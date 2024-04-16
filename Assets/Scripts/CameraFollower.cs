using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform cam;
    public Transform player;
    public float CamDistance;

    // Update is called once per frame
    void Update()
    {
        cam.position = player.position + new Vector3(CamDistance, 5, 0);
    }
}
