using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameNameLevel;
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject Option;
    [SerializeField] private GameObject Credit;
    [SerializeField] private GameObject[] MainButton;
    [SerializeField] private GameObject Resolution;
    [SerializeField] private GameObject Sound;

    void Start()
    {
        StartMainButton();
    }

    public void Play()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void NewGame()
    {
        SceneManager.LoadScene("characterCreation");
    }

    public void Options()
    {
        Option.SetActive(true);
        DesableMainButton();
    }

    public void CloseOptions()
    {
        Option.SetActive(false);
        StartMainButton();
    }

    public void Creditos()
    {
        Credit.SetActive(true);
        DesableMainButton();
    }

    public void CloseCreditos()
    {
        Credit.SetActive(false);
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

    public void LeaveGame()
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
