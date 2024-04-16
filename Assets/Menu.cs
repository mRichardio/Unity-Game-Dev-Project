using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    public Button DefaultButton;
    public GameObject Music;
    public SaveManager saveManager;

    // Start is called before the first frame update
    void Start()
    {
        saveManager.LoadGame();
        SaveData saveData = saveManager.GetSaveData(); // Access loaded SaveData
        Debug.Log("Level Complete: " + saveData.Level1Complete);
        DefaultButton.Select();
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        DontDestroyOnLoad(Music);
        DontDestroyOnLoad(saveManager);
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
