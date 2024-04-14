using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrefabBuilder : MonoBehaviour
{
    // REFERENCE
    // Tutorial: https://www.youtube.com/watch?v=JfpMIUDa-Mk
    // Documentation: https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html && https://docs.unity3d.com/ScriptReference/Physics.Raycast.html

    PlayerController player;
    Tower tower;

    public GameObject Tower = null;
    public GameObject PreviewTowerPrefab = null;
    GameObject previewTowerInstance = null;
    Camera cam = null;

    private float towerOffset = 0.75f;

    // Parent object for towers
    public Transform TowerParent;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        tower = Tower.GetComponent<Tower>();
    }

    // Update is called once per frame
    void Update()
    {
        PreviewAtMousePos();
        SpawnAtMousePos();
    }

    void SpawnAtMousePos()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (player.CurrentMoney >= tower.price)
            {
                player.CurrentMoney -= tower.price;

                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;

                int layerMask = ~LayerMask.GetMask("Player");

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    Destroy(previewTowerInstance);
                    Instantiate(Tower, new Vector3(hit.point.x, hit.point.y + towerOffset, hit.point.z), Quaternion.identity, TowerParent);
                    Tower t = Tower.GetComponent<Tower>();
                    t.ShrinkSpeed = Random.Range(0.2f, 0.5f);
                }
            }
            else
            {
                // Make error sound or text
            }
        }
    }

    void PreviewAtMousePos()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        // Reference for layer mask: https://discussions.unity.com/t/how-can-i-have-a-raycast-ignore-a-layer-completely/116196/2
        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity,layerMask))
        {
            if (previewTowerInstance == null)
            {
                previewTowerInstance = Instantiate(PreviewTowerPrefab, new Vector3(hit.point.x, hit.point.y + towerOffset, hit.point.z), Quaternion.identity, TowerParent);
            }
            else
            {
                previewTowerInstance.transform.position = new Vector3(hit.point.x, hit.point.y + towerOffset, hit.point.z);
            }
        }
        else
        {
            if (previewTowerInstance != null)
            {
                Destroy(previewTowerInstance);
                previewTowerInstance = null;
            }
        }
    }
}
