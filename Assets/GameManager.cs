using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Stats
    PlayerController playerController;
    Crystal crystal;
    Menu menu;

    // Sounds
    public AudioClip GameOverSound;
    public AudioClip GameCompleteSound;

    // Score
    public float Score;

    public int TowerCount;

    // Parents
    public GameObject SpawnPoint_A;
    public GameObject SpawnPoint_B;
    public GameObject SpawnPoint_C;
    public GameObject SpawnPoint_D;

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
    public bool IsPaused;
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

        Time.timeScale = 1;

        WaveText.gameObject.active = false;
        PreperationText.gameObject.active = false;

        // Set spawn time
        nextSpawnTime = Time.time + SpawnInterval;

        // Gets the player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Save Manager // Save Manager is in dontdestroyonload
        saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();

        // Music
        PrepMusic = GameObject.Find("Music").GetComponent<AudioSource>();
        PlayMusic = GameObject.Find("BattleMusic").GetComponent<AudioSource>();

        // Crystal
        crystal = GameObject.Find("Crystal").GetComponent<Crystal>();
    }

    void Awake()
    {
        Enemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
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
                    PreperationText.text = "You lose... Your crystal died.";

                    IsPaused = true;
                    PauseCanvas.SetActive(IsPaused);

                    // Play game over sound
                    AudioSource.PlayClipAtPoint(GameOverSound, Camera.main.transform.position);

                    Time.timeScale = 0;
                    PauseText.text = "Game Over!, Your crystal died....";
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
                    WaveText.text = "You lose... Your crystal died.";
                    IsPaused = true;
                    PauseCanvas.SetActive(IsPaused);

                    // Play game over sound
                    AudioSource.PlayClipAtPoint(GameOverSound, transform.position);

                    Time.timeScale = 0;
                    PauseText.text = "Game Over!, Your crystal died....";
                    playerController.Sensitivity = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        // Spawn an enemy
        if (isPlaying && EnemiesSpawnedThisWave < WaveLightEnemyCount + WaveBasicEnemyCount + WaveHeavyEnemyCount)
        {
            if (Time.time >= nextSpawnTime)
            {
                // Spawn enemy types randomly
                int spawnType = Random.Range(0, 3);

                bool spawnA = false, spawnB = false;

                switch (spawnType)
                {
                    case 0:
                        if (countBasicEnemiesSpawned < WaveBasicEnemyCount)
                        {
                            spawnA = SpawnEnemy("Basic", SpawnPoint_A);
                            spawnB = SpawnEnemy("Basic", SpawnPoint_B);
                        }
                        break;
                    case 1:
                        if (countLightEnemiesSpawned < WaveLightEnemyCount)
                        {
                            spawnA = SpawnEnemy("Light", SpawnPoint_A);
                            spawnB = SpawnEnemy("Light", SpawnPoint_B);
                        }
                        break;
                    case 2:
                        if (countHeavyEnemiesSpawned < WaveHeavyEnemyCount)
                        {
                            spawnA = SpawnEnemy("Heavy", SpawnPoint_A);
                            spawnB = SpawnEnemy("Heavy", SpawnPoint_B);
                        }
                        break;
                }

                // Update next spawn time if any enemy was spawned
                if (spawnA || spawnB)
                {
                    nextSpawnTime = Time.time + SpawnInterval;
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

    // Returns true if an enemy is spawned
    public bool SpawnEnemy(string type, GameObject spawnPoint)
    {
        if (type == "Basic" && countBasicEnemiesSpawned < WaveBasicEnemyCount)
        {
            // Instantiate the enemy at spawnPoint
            GameObject enemy = Instantiate(BasicEnemyPrefab, spawnPoint.transform.position, Quaternion.identity, EnemyParent.transform);
            Debug.Log("Spawning " + type + " at " + spawnPoint.name);
            enemy.name = "Enemy Basic";
            Enemies.Add(enemy);

            // Update counters
            EnemiesSpawnedThisWave++;
            countBasicEnemiesSpawned++;
            return true; // Indicate that an enemy was spawned
        }
        if (type == "Light")
        {
            if (Time.time >= nextSpawnTime)
            {
                // Instantiate the enemy
                GameObject enemy = Instantiate(LightEnemyPrefab, spawnPoint.transform.position, Quaternion.identity, EnemyParent.transform);
                Debug.Log("Spawning " + type + " at " + spawnPoint.name);
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
                Debug.Log("Spawning " + type + " at " + spawnPoint.name);
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

        return false; // No enemy spawned
    }

    public void PauseHandler()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            IsPaused = !IsPaused;
            PauseCanvas.SetActive(IsPaused);

            Time.timeScale = IsPaused ? 0 : 1;

            // Switch sensitivity between 4 and 0
            playerController.Sensitivity = IsPaused ? 0 : 4;
            Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = IsPaused ? true : false;
        }
    }

    private void InitialiseWaves()
    {
        if (Wave == 1)
        {
            SetWaveEnemyCount("Light", 10);
            SetWaveEnemyCount("Basic", 10);
            SetWaveEnemyCount("Heavy", 10);
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

                WaveText.text = "Level Complete";
                IsPaused = true;
                PauseCanvas.SetActive(IsPaused);

                // Play level complete sound
                AudioSource.PlayClipAtPoint(GameCompleteSound, transform.position);
                Time.timeScale = 0;
                PauseText.text = "Level Complete!";
                //playerController.GetComponent<PlayerController>().enabled = false;
                playerController.Sensitivity = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

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
        if (isPlaying || isFightingBoss)
        {
            PrepMusic.enabled = false;
            PlayMusic.enabled = true;
        }
    }

    public void LoadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);

        // Destroy Loaded Objects
        Destroy(GameObject.Find("Music"));
        Destroy(GameObject.Find("BattleMusic"));
        Destroy(GameObject.Find("Save Manager"));
        Destroy(GameObject.Find("Menu Manager"));
    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
                // Stops play mode in the Unity editor
                EditorApplication.isPlaying = false;
        #endif
    }
}
