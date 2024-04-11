using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class BillboardGeneral : MonoBehaviour
{
    public Transform cam;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        //Debug.Log("Player" + player);
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        cam = playerCamera.transform;
    }

    private void Update()
    {
        if (cam == null)
        {
            player = GameObject.Find("Player");
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            cam = playerCamera.transform;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
