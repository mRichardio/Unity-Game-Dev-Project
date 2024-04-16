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

    // Start is called before the first frame update
    void Start()
    {
        DefaultButton.Select();
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        DontDestroyOnLoad(Music);
    }

    public void Quit()
    {
        Application.Quit();
        // Need an assembly reference to UnityEditor
        #if UNITY_EDITOR
                // Stops play mode in the Unity editor
                EditorApplication.isPlaying = false;
        #endif


    }
}
