using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform Cam;
    public Transform BuildCam;
    private GameObject player;
    private GameObject buildCamParent;

    private void Start()
    {
        buildCamParent = GameObject.Find("BuildCameraParent");
        Camera _BuildCam = buildCamParent.GetComponentInChildren<Camera>();
        BuildCam = _BuildCam.transform;

        // only if build cam isnt active
        if (!BuildCam.transform.gameObject.active)
        {
            player = GameObject.Find("Player");
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            Cam = playerCamera.transform;
        }
    }

    void Update()
    {
        // only if cam is active
        if (!BuildCam.transform.gameObject.active)
        {
            player = GameObject.Find("Player");
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            Cam = playerCamera.transform;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // only if cam is active
        if (!BuildCam.transform.gameObject.active)
        {
            transform.LookAt(transform.position + Cam.forward);        
        }
    }
}
