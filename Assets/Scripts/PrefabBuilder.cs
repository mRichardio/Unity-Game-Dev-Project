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
    public int CurrentTowerCount;
    public int MaxTowerCount;
    public GameObject TowerCountText;

    public bool isInBuildMode;

    // Selectable Towers
    public GameObject Tower_A;
    public GameObject Tower_B;
    public GameObject Tower_C;

    // Preview Towers
    public GameObject PreviewTower_A;
    public GameObject PreviewTower_B;
    public GameObject PreviewTower_C;

    public GameObject PreviewTowerPrefab = null; // Using this temp until unique towers have been made
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
        SelectTower();
        PreviewAtMousePos();
        SpawnAtMousePos();
    }

    void SpawnAtMousePos()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (player.CurrentMoney >= tower.price && CurrentTowerCount < MaxTowerCount)
            {
                CurrentTowerCount++;
                TowerCountText.GetComponent<TMPro.TextMeshProUGUI>().text = CurrentTowerCount + "/" + MaxTowerCount;
                
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
            if (isInBuildMode)
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

    // Take Keypress 1-3 and select the tower
    public void SelectTower()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UnityEngine.Debug.Log("Tower A Selected");
            if (PreviewTowerPrefab != PreviewTower_A)
            {
                ResetPreviewTower();
                PreviewTowerPrefab = PreviewTower_A;
                Tower = Tower_A;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UnityEngine.Debug.Log("Tower B Selected");
            if (PreviewTowerPrefab != PreviewTower_B)
            {
                ResetPreviewTower();
                PreviewTowerPrefab = PreviewTower_B;
                Tower = Tower_B;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UnityEngine.Debug.Log("Tower C Selected");
            if (PreviewTowerPrefab != PreviewTower_C)
            {
                ResetPreviewTower();
                PreviewTowerPrefab = PreviewTower_C;
                Tower = Tower_C;
            }
        }
    }

    public void ResetPreviewTower()
    {
        if (previewTowerInstance != null)
        {
            Destroy(previewTowerInstance);
            previewTowerInstance = null;
        }
    }
}
