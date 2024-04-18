using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    public Button DefaultButton;
    public GameObject Music;
    public GameObject BattleMusic;
    public SaveManager saveManager;
    public GameObject MenuManager;
    public TextMeshProUGUI Level1CompleteText;
    public TextMeshProUGUI Level1ScoreText;

    // Start is called before the first frame update
    void Start()
    {
        saveManager.LoadGame();
        SaveData saveData = saveManager.GetSaveData(); // Access loaded SaveData
        if (saveData != null)
        {
            if (saveData.Level1Complete)
            {
                Level1CompleteText.text = "Complete";
                Level1ScoreText.text = "Highscore: " + saveData.Level1HighScore;
            }
            Debug.Log("Level Complete: " + saveData.Level1Complete);
        }
        DefaultButton.Select();
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        DontDestroyOnLoad(Music);
        DontDestroyOnLoad(BattleMusic);
        DontDestroyOnLoad(saveManager);
        DontDestroyOnLoad(MenuManager);
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
