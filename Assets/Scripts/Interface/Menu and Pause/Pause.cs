using UnityEngine;
using UnityEngine.SceneManagement;
using DialogueEditor;
public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] private GameObject Option;
    [SerializeField] private GameObject Resolution;
    [SerializeField] private GameObject Sound;
    [SerializeField] private GameObject ExitGame;
    [SerializeField] private GameObject[] MainButton;
    [SerializeField] private ScreenController screenController; // Arraste o ScreenController no Inspector

    public DialogNPC dialogNPC; // Referência para o script de diálogo (assegure-se de arrastar no inspector)

    private void Start()
    {
        Pause.GameIsPaused = false; // Reseta o estado de pausa ao carregar a cena
        Time.timeScale = 1f; // Certifica que o jogo não está pausado
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Fecha qualquer interface aberta
            CloseActiveUI();

            if (AnyOptionActive())
            {
                // Fecha qualquer menu aberto (Opções, Resolução, Som, Sair)
                CloseAllOptions();
            }
            else
            {
                // Alterna entre pausar e retomar o jogo
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    PauseGame();
                }
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

    public void OpenExitGame()
    {
        ExitGame.SetActive(true);
        DesableMainButton();
    }

    public void CloseExitGame()
    {
        ExitGame.SetActive(false);
        StartMainButton();
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

    private void CloseActiveUI()
    {
        if (screenController.Inventory.activeSelf)
        {
            screenController.Inventory.SetActive(false);
        }
        if (screenController.Attributes.activeSelf)
        {
            screenController.Attributes.SetActive(false);
        }
        if (screenController.Daily.activeSelf)
        {
            screenController.Daily.SetActive(false);
        }
        if (screenController.Map.activeSelf)
        {
            screenController.Map.SetActive(false);
        }
    }

    private bool AnyOptionActive()
    {
        return Option.activeSelf || Resolution.activeSelf || Sound.activeSelf || ExitGame.activeSelf;
    }

    private void CloseAllOptions()
    {
        Option.SetActive(false);
        Resolution.SetActive(false);
        Sound.SetActive(false);
        ExitGame.SetActive(false);
        StartMainButton(); // Reativa os botões principais
    }
}
