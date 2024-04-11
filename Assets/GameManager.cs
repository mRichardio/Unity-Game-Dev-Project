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

    // Spawn
    public float SpawnInterval;
    private float nextSpawnTime;

    // Waves
    public int Wave;
    private int WaveEnemyCount;
    private int EnemiesSpawnedThisWave;
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
        Enemies = new List<GameObject>();
        isPreparing = true;
        isPlaying = false;

        WaveText.gameObject.active = false;
        PreperationText.gameObject.active = false;

        // Set spawn time
        nextSpawnTime = Time.time + SpawnInterval;

        SetWaveEnemyCount(1);
    }

    void Awake()
    {
        Enemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Dev Tools
        {
            // Ready Up
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (isPreparing && !isPlaying) // Check if the wave is ready to start
                {
                    isPreparing = false;
                    isPlaying = true;
                    Debug.Log("Playing, Wave:  " + Wave);
                    
                    EnemiesSpawnedThisWave = 0; // Reset the enemies spawned tracker
                    // I can do things like setting wave-specific conditions here, such as increasing enemy count, changing types, etc....
                }
            }

            // Press a button to check state
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (isPreparing)
                {
                    Debug.Log("Preparing, Wave:  " + Wave); // Check state
                    PreperationText.gameObject.active = true;
                    WaveText.gameObject.active = false;
                }
                else if (isPlaying)
                {
                    Debug.Log("Playing, Wave:  " + Wave); // Check state
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
            // UI Text Toggle
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
            if (isPlaying && EnemiesSpawnedThisWave < WaveEnemyCount)
            {
                SpawnEnemy("Basic", SpawnPoint_A);
            }
        }

        // Next Wave
        {
            NextWave();
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
                enemy.name = "Enemy"; // Might wanna change this to basic, medium etc.
                Enemies.Add(enemy); // Might need to add a check to see if the enemy is dead and remove it from the list
                nextSpawnTime = Time.time + SpawnInterval;
                EnemiesSpawnedThisWave++;
            }
        }
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

    public void NextWave()
    {
        if (Enemies.Count == 0 && EnemiesSpawnedThisWave == WaveEnemyCount && !isPreparing)
        {
            Wave++;
            WaveText.text = "Wave " + Wave;
            isPreparing = true;
            isPlaying = false;
        }
    }

    public void SetWaveEnemyCount(int count)
    {
        WaveEnemyCount = count;
    }
}
