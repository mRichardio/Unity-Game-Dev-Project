using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject ShopPanel;
    public Canvas UICanvas;

    public Canvas shopCanvas;

    // Start is called before the first frame update
    void Start()
    {
        GameObject UICanvasObject = GameObject.Find("UI");
        if (UICanvasObject != null)
        {
            UICanvas = UICanvasObject.GetComponent<Canvas>();
        }

        Transform shopPanelTransform = UICanvasObject.transform.Find("ShopPanel");
        if (shopPanelTransform != null)
        {
            ShopPanel = shopPanelTransform.gameObject;
        }

        // UI Event Camera
        GameObject player = GameObject.Find("Player");
        GameObject playerCameraObj = player.transform.Find("PlayerCamera").gameObject;
        Camera playerCamera = playerCameraObj.GetComponent<Camera>();
        shopCanvas.worldCamera = playerCamera;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayShop()
    {
        if (ShopPanel != null)
        {
            ShopPanel.SetActive(!ShopPanel.activeSelf);
        }
    }
}
