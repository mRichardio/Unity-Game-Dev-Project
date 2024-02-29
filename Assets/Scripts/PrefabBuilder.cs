using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrefabBuilder : MonoBehaviour
{
    // REFERENCE
    // Tutorial: https://www.youtube.com/watch?v=JfpMIUDa-Mk
    // Documentation: https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html && https://docs.unity3d.com/ScriptReference/Physics.Raycast.html

    public GameObject tower = null;
    Camera cam = null;

    // Parent object for towers
    public Transform TowerParent;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;    
    }

    // Update is called once per frame
    void Update()
    {
        SpawnAtMousePos();
    }

    void SpawnAtMousePos()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) 
            {
                Instantiate(tower, new Vector3(hit.point.x, hit.point.y + tower.transform.position.y/2, hit.point.z), Quaternion.identity, TowerParent);
                Tower t = tower.GetComponent<Tower>();
                t.ShrinkSpeed = Random.Range(0.2f, 0.5f);
            }
        }
    }
}
