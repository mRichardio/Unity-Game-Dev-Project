using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Parents
    public GameObject SpawnPoint_A;
    public GameObject SpawnPoint_B;
    public GameObject SpawnPoint_C;
    public GameObject SpawnPoint_D;

    // Prefab Parents
    public GameObject EnemyParent;
    
    // Prefabs
    public GameObject BasicEnemyPrefab;

    // Collections
    public List<GameObject> Enemies = new List<GameObject>();

    // Spawn Interval
    public float SpawnInterval;
    private float nextSpawnTime;

    // Waves
    public int Wave;
    private bool isPreparing;
    private bool isPlaying;

    // Round UI
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI PreperationText;

    // Pause
    bool IsPaused;
    public GameObject PauseCanvas;

    // Start is called before the first frame update
    void Start()
    {
        isPreparing = true;
        isPlaying = false;

        WaveText.gameObject.active = false;
        PreperationText.gameObject.active = false;

        // Set spawn time
        nextSpawnTime = Time.time + SpawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Dev Tools
        {
            // Ready Up // TEMP A SWITCH BUT ROUNDS WILL END WHEN ENEMY COUNT IS 0 AND WILL BEGIN WHEN G IS PRESSED
            if (isPreparing)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    isPreparing = false;
                    isPlaying = true;
                }
            }
            else if (isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    isPreparing = true;
                    isPlaying = false;
                }
            }

            // Press a button to check state
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (isPreparing)
                {
                    Debug.Log("Preparing");
                    PreperationText.gameObject.active = true;
                    WaveText.gameObject.active = false;
                }
                else if (isPlaying)
                {
                    Debug.Log("Playing");
                    PreperationText.gameObject.active = false;
                    WaveText.gameObject.active = true;
                }
            }
        }

        // Pause/Unpause
        {
            PauseHandler();
        }

        {
            // UI Text
            if (isPreparing)
            {
                PreperationText.gameObject.active = true;
                WaveText.gameObject.active = false;
            }
            else if (isPlaying)
            {
                PreperationText.gameObject.active = false;
                WaveText.gameObject.active = true;
            }
        }

        // Spawn an enemy
        {
            if (isPlaying)
            {
                SpawnEnemy("Basic", SpawnPoint_A);
            }
        }
    }

    public void SpawnEnemy(string type, GameObject spawnPoint)
    {
        if (type == "Basic")
        {
            if (Time.time >= nextSpawnTime)
            {
                // Instantiate the enemy
                GameObject enemy = Instantiate(BasicEnemyPrefab, spawnPoint.transform.position, Quaternion.identity, EnemyParent.transform);
                Enemies.Add(enemy); // Might need to add a check to see if the enemy is dead and remove it from the list
                nextSpawnTime = Time.time + SpawnInterval;
            }
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        Enemies.Remove(enemy);
    }   

    public void PauseHandler()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            IsPaused = !IsPaused;
            PauseCanvas.SetActive(IsPaused);

            Time.timeScale = IsPaused ? 0 : 1;
        }
    }
}
