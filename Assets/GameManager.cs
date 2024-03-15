using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private CameraControl cameraControl;
    private PlayerHealth playerHealth;
    private FootstepManager footstepManager;
    private WeaponManager weaponManager;
    private BuildMode buildMode;

    public int playerPrestige;
    public Button shopBtn;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        cameraControl = GetComponent<CameraControl>();
        playerHealth = GetComponent<PlayerHealth>();
        footstepManager = GetComponent<FootstepManager>();
        weaponManager = GetComponent<WeaponManager>();
        buildMode = GetComponent<BuildMode>();

        playerPrestige = 0;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        // FixedUpdate methods of smaller components can be called here
        // For example:
        // playerMovement.HandleJumping();
        // playerMovement.HandleSprinting();
        // footstepManager.PlayFootstepSoundAndCreateVisuals();
    }

    public void PrestigePlayer(int upgAmount)
    {
        // Player Prestige
        if (playerPrestige >= 2)
        {
            shopBtn.interactable = false;
            shopBtn.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Max Prestige";
        }
        else
        {
            playerPrestige += upgAmount;
        }
    }

    // Make a function to edit player health, movement etc based on prestige.../.....
}
