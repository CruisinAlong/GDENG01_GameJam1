using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void OnStartButton()
    {
        SceneManager.LoadScene("InitialScene");
    }

    public void OnControls()
    {
        SceneManager.LoadScene("ControlsScene");
    }

    public void OnGameOver_Menu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }




    public void OnExitButton()
    {
        Application.Quit();
    }
}
