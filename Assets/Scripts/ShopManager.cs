using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject ShopPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayShop()
    {
        if (ShopPanel != null)
        {
            Debug.Log(ShopPanel.name);
            ShopPanel.SetActive(!ShopPanel.activeSelf);
        }
    }
}
