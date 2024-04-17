using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Stats
    PlayerController playerController;
    Crystal crystal;

    // Score
    public float Score;

    public int TowerCount;

    // Parents
    public GameObject SpawnPoint_A;
    public GameObject SpawnPoint_B;
    public GameObject SpawnPoint_C;
    public GameObject SpawnPoint_D;

    // Shader
    public Material GridMaterial;
    private float duration = 2.0f; // Duration of the transition in seconds
    private float initialThickness = 0.0003f;
    private float targetThickness = 0.1f;
    private float elapsed = 0f;

    // Prefab Parents
    public GameObject EnemyParent;
    
    // Prefabs
    public GameObject LightEnemyPrefab;
    public GameObject BasicEnemyPrefab;
    public GameObject HeavyEnemyPrefab;

    public GameObject BossEnemyPrefab;

    // Collections
    public List<GameObject> Enemies = new List<GameObject>();

    // Spawn
    public float SpawnInterval;
    private float nextSpawnTime;

    // Waves
    public int Wave;
    public int WaveCap;
    public bool GameOver;
    public bool isFightingBoss;
    public bool BossDefeated;

    private int WaveLightEnemyCount; // Limits
    private int WaveBasicEnemyCount;
    private int WaveHeavyEnemyCount;

    private int countBasicEnemiesSpawned = 0; // Count Tracker
    private int countHeavyEnemiesSpawned = 0;
    private int countLightEnemiesSpawned = 0;

    private int EnemiesSpawnedThisWave;
    public bool isPreparing { get; set; }
    public bool isPlaying { get; set;}

    // Round UI
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI PreperationText;
    public TextMeshProUGUI PauseText;

    // HUD UI
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI TowerCountText;
    public GameObject BuildPanel;

    // Music
    public AudioSource PrepMusic;
    public AudioSource PlayMusic;

    // Pause
    bool IsPaused;
    public GameObject PauseCanvas;

    // Save
    public SaveManager saveManager;

    // Start is called before the first frame update
    void Start()
    {
        Enemies = new List<GameObject>();
        isPreparing = true;
        isPlaying = false;
        isFightingBoss = false;

        WaveText.gameObject.active = false;
        PreperationText.gameObject.active = false;

        // Set spawn time
        nextSpawnTime = Time.time + SpawnInterval;

        // Set the initial grid line thickness
        GridMaterial.SetFloat("_GridLineThickness", initialThickness);

        // Gets the player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Save Manager // Save Manager is in dontdestroyonload
        saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();

        PrepMusic = GameObject.Find("Music").GetComponent<AudioSource>();
        PlayMusic = GameObject.Find("BattleMusic").GetComponent<AudioSource>();

        crystal = GameObject.Find("Crystal").GetComponent<Crystal>();
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

        // Update UI
        {
            UpdateUI();
        }

        // Ready Up
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isPreparing && !isPlaying && !GameOver) // Check if the wave is ready to start
            {
                isPreparing = false;
                isPlaying = true;
                Debug.Log("Playing, Wave:  " + Wave);

                EnemiesSpawnedThisWave = 0; // Reset the enemies spawned tracker

                InitialiseWaves(); // Sets up the parameters for the waves
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
                if (crystal.isAlive == false)
                {
                    Debug.Log("herro 1 " + crystal.isAlive);
                    PreperationText.text = "You lose... Your crystal died.";

                    IsPaused = true;
                    PauseCanvas.SetActive(IsPaused);

                    Time.timeScale = 0;
                    PauseText.text = "Game Over!, Your crystal died....";
                    //playerController.GetComponent<PlayerCokntroller>().enabled = false;
                    playerController.Sensitivity = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            if (isPlaying)
            {
                PreperationText.gameObject.active = false;
                WaveText.gameObject.active = true;
                if (crystal.isAlive == false)
                {
                    Debug.Log("herro 2 " + crystal.isAlive);
                    WaveText.text = "You lose... Your crystal died.";
                    IsPaused = true;
                    PauseCanvas.SetActive(IsPaused);

                    Time.timeScale = 0;
                    PauseText.text = "Game Over!, Your crystal died....";
                    //playerController.GetComponent<PlayerController>().enabled = false;
                    playerController.Sensitivity = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        // Spawn an enemy
        {
            if (isPlaying && EnemiesSpawnedThisWave < WaveLightEnemyCount + WaveBasicEnemyCount + WaveHeavyEnemyCount)
            {
                // Spawn enemy types randomly
                int spawnType = Random.Range(0, 3);

                switch (spawnType)
                {
                    case 0:
                        if (countBasicEnemiesSpawned < WaveBasicEnemyCount)
                        {
                            SpawnEnemy("Basic", SpawnPoint_A);
                        }
                        break;
                    case 1:
                        if (countLightEnemiesSpawned < WaveLightEnemyCount)
                        {
                            SpawnEnemy("Light", SpawnPoint_A);
                        }
                        break;
                    case 2:
                        if (countHeavyEnemiesSpawned < WaveHeavyEnemyCount)
                        {
                            SpawnEnemy("Heavy", SpawnPoint_A);
                        }
                        break;
                }
            }
        }

        // Next Wave
        {
            NextWave();
            BossCheck();    
            UpdateMusic();
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
                enemy.name = "Enemy Basic";
                Enemies.Add(enemy);
                nextSpawnTime = Time.time + SpawnInterval;

                // Overall Couter
                EnemiesSpawnedThisWave++;
                // Type Counter
                countBasicEnemiesSpawned++;
            }
        }
        if (type == "Light")
        {
            if (Time.time >= nextSpawnTime)
            {
                // Instantiate the enemy
                GameObject enemy = Instantiate(LightEnemyPrefab, spawnPoint.transform.position, Quaternion.identity, EnemyParent.transform);
                enemy.name = "Enemy Light";
                Enemies.Add(enemy);
                nextSpawnTime = Time.time + SpawnInterval;
                
                // Overall Couter
                EnemiesSpawnedThisWave++;
                // Type Counter
                countLightEnemiesSpawned++;
            }
        }
        if (type == "Heavy")
        {
            if (Time.time >= nextSpawnTime)
            {
                // Instantiate the enemy
                GameObject enemy = Instantiate(HeavyEnemyPrefab, spawnPoint.transform.position, Quaternion.identity, EnemyParent.transform);
                enemy.name = "Enemy Heavy";
                Enemies.Add(enemy);
                nextSpawnTime = Time.time + SpawnInterval;

                // Overall Couter
                EnemiesSpawnedThisWave++;
                // Type Counter
                countHeavyEnemiesSpawned++;
            }
        }

        if (type == "Boss")
        {
            if (Time.time >= nextSpawnTime)
            {
                // Instantiate the enemy
                GameObject enemy = Instantiate(BossEnemyPrefab, spawnPoint.transform.position, Quaternion.identity, EnemyParent.transform);
                enemy.name = "Enemy Boss";
                Enemies.Add(enemy);
                nextSpawnTime = Time.time + SpawnInterval;

                // Overall Couter
                EnemiesSpawnedThisWave++;
                // Type Counter
                countHeavyEnemiesSpawned++;
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

    private void InitialiseWaves()
    {
        if (Wave == 1)
        {
            SetWaveEnemyCount("Light", 1);
            SetWaveEnemyCount("Basic", 1);
            SetWaveEnemyCount("Heavy", 1);

            // need to add other enemy types
        }
        else if (Wave == 2)
        {
            SetWaveEnemyCount("Light", 10);
            SetWaveEnemyCount("Basic", 10);
            SetWaveEnemyCount("Heavy", 10);
        }
        else if (Wave == 3)
        {
            SetWaveEnemyCount("Light", 15);
            SetWaveEnemyCount("Basic", 15);
            SetWaveEnemyCount("Heavy", 15);
        }
        else if (Wave == 4)
        {
            SetWaveEnemyCount("Light", 20);
            SetWaveEnemyCount("Basic", 20);
            SetWaveEnemyCount("Heavy", 20);
        }
        else if (Wave == 5)
        {
            SetWaveEnemyCount("Light", 25);
            SetWaveEnemyCount("Basic", 25);
            SetWaveEnemyCount("Heavy", 25);
        }
    }

    public void NextWave()
    {
        //HandleGridLineThickness();
        if (Enemies.Count == 0 && EnemiesSpawnedThisWave == WaveLightEnemyCount + WaveBasicEnemyCount + WaveHeavyEnemyCount && !isPreparing && GameOver == false)
        {
            Wave++;
            WaveText.text = "Wave " + Wave;
            isPreparing = true;
            isPlaying = false;

            if (Wave > WaveCap)
            {
                isFightingBoss = true;
                GameOver = true;
                PreperationText.text = "Boss Fight!";
                SpawnEnemy("Boss", SpawnPoint_A);
                Debug.Log("Game Over"); 
            }
        }
    }

    public void SetWaveEnemyCount(string type, int count)
    {
        if (type == "Basic")
        {
            WaveBasicEnemyCount = count;
        }
        if (type == "Light")
        {
            WaveLightEnemyCount = count;
        }
        if (type == "Heavy")
        {
            WaveHeavyEnemyCount = count;
        }
    }

    // Check if the boss has been defeated
    void BossCheck()
    {
        if (Wave >= WaveCap)
        {
            // Check if the boss has been defeated
            if (Enemies.Count == 0 && BossDefeated)
            {
                // Game Over
                GameOver = true;
                PreperationText.text = "Game Over!";
                Debug.Log("Game Over");
                if (playerController.currentHealth > 0)
                {
                    Score = playerController.CurrentMoney * 13; // Score Calculation (It's not amazing but should do the job)
                    saveManager.CompleteLevel(1, Score);
                }
                else { saveManager.CompleteLevel(1, 100); } // Handles save

            }
        }
    }

    public void UpdateUI()
    {
        HealthText.text = crystal.GetHealth().ToString();
        MoneyText.text = playerController.CurrentMoney.ToString();
        // NEED TO ADD TOWER COUNT
        if (isPreparing && !isFightingBoss)
        {
            BuildPanel.SetActive(true);
        }
        else {  BuildPanel.SetActive(false); }
    }

    public void UpdateMusic()
    {
        if (isPreparing && !isFightingBoss)
        {
            PrepMusic.enabled = true;
            PlayMusic.enabled = false;
        }
        if (isPlaying && isFightingBoss)
        {
            PrepMusic.enabled = false;
            PlayMusic.enabled = true;
        }
    }
}
