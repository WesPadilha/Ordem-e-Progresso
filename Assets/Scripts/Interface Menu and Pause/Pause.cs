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

    public DialogNPC dialogNPC; // Referência para o script de diálogo (assegure-se de arrastar no inspector)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (dialogNPC != null && ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
            {
                // Se uma conversa estiver ativa, apenas encerre a conversa
                ConversationManager.Instance.EndConversation();
                dialogNPC.EndConversation(); // Certifique-se de que o método EndConversation do DialogNPC seja chamado
            }
            else
            {
                // Se não houver conversa ativa, alterna entre pausar e retomar o jogo
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
}

