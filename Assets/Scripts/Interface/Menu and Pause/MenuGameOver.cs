using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Certifique-se de incluir o namespace correto para UI

public class MenuGameOver : MonoBehaviour
{
    public GameObject ExitGame;
    public GameObject Load;
    public void OpenLoad()
    {
        Load.SetActive(true);
    }

    public void CloseLoad()
    {
        Load.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenExitGame()
    {
        ExitGame.SetActive(true);
    }

    public void CloseExitGame()
    {
        ExitGame.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
