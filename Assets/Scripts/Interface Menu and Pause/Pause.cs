using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] private GameObject Option;
    [SerializeField] private GameObject Resolution;
    [SerializeField] private GameObject Sound;
    [SerializeField] private GameObject[] MainButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                PauseGame();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void OpenOptions()
    {
        Option.SetActive(true);
        DesableMainButton();
    }

    public void CloseOptions()
    {
        Option.SetActive(false);
        StartMainButton();
    }

    public void OpenResolution()
    {
        if (Sound.activeSelf)
        {
            Sound.SetActive(false);
        }
        
        Resolution.SetActive(true);
        DesableMainButton();
    }

    public void CloseResolution()
    {
        Resolution.SetActive(false);
    }

    public void OpenSound()
    {
        if (Resolution.activeSelf)
        {
            Resolution.SetActive(false);
        }

        Sound.SetActive(true);
        DesableMainButton();
    }

    public void CloseSound()
    {
        Sound.SetActive(false);
    }

    public void LoadMenu()

    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void DesableMainButton()
    {
        foreach (GameObject button in MainButton)
        {
            button.SetActive(false);
        }
    }
    private void StartMainButton()
    {
        foreach (GameObject button in MainButton)
        {
            button.SetActive(true);
        }
    }
}

