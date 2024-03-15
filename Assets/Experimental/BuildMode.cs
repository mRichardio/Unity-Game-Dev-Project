using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : MonoBehaviour
{
    public GameObject BuildCamera;
    public GameObject PlayerCamera;

    private void Start()
    {
        BuildCamera.SetActive(false);
        PlayerCamera.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleBuildMode();
        }
    }

    public void ToggleBuildMode()
    {
        if (BuildCamera.activeSelf == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            BuildCamera.SetActive(false);
            PlayerCamera.SetActive(true);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            BuildCamera.SetActive(true);
            PlayerCamera.SetActive(false);
        }
    }

    // Other methods as needed
}
