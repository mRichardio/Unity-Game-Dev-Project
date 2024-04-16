using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData
{
    public bool Level1Complete;
    public bool Level2Complete;
    public float Level1HighScore;
    public float Level2HighScore;
}

public class SaveManager : MonoBehaviour
{
    private SaveData saveData = new SaveData();
    private string saveFilePath;

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "Save.json");
        LoadGame();
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No save file found");
        }
    }

    public void CompleteLevel(int level, float score)
    {
        if (level == 1)
        {
            saveData.Level1Complete = true;
            saveData.Level1HighScore = Mathf.Max(saveData.Level1HighScore, score);
        }
        else if (level == 2)
        {
            saveData.Level2Complete = true;
            saveData.Level2HighScore = Mathf.Max(saveData.Level2HighScore, score);
        }

        SaveGame();
    }
}

