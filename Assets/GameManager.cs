using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Parents
    public GameObject enemyParent;

    // Prefabs
    public GameObject enemyPrefab;

    // Spawn Interval
    public float SpawnInterval = 3.0f;
    private float nextSpawnTime = 0.0f;

    // Rounds
    public bool isPreparing;
    public bool isPlaying;


    // Start is called before the first frame update
    void Start()
    {
        isPreparing = true;
        isPlaying = false;

        // Set spawn time
        nextSpawnTime = Time.time + SpawnInterval;
    }

    // Update is called once per frame
    void Update()
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
            }
            else if (isPlaying)
            {
                Debug.Log("Playing");
            }
        }

        // Spawn an enemy
        if (isPlaying)
        {
            SpawnEnemy("Basic");
        }
    }

    public void SpawnEnemy(string type)
    {
        if (type == "Basic")
        {
            if (Time.time >= nextSpawnTime)
            {
                // Instantiate the enemy
                GameObject enemy = Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity, enemyParent.transform);
                nextSpawnTime = Time.time + SpawnInterval;
            }
        }
    }
}
