using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        UnityEditor.EditorApplication.isPlaying = false; // Debug
    }
}
