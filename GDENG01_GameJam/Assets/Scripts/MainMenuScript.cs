using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void OnStartButton()
    {
        LoadManager.Instance.LoadScene(SceneNames.GAME_SCENE);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
