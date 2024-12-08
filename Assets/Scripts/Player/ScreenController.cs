using UnityEngine;
using DialogueEditor;


public class ScreenController : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject Attributes;
    public GameObject Daily;
    public GameObject Map;
    public MainCameraController mainCameraController;   
    private bool isConversationActive = false; // Acompanhar se a conversa está ativa

    public bool IsAnyUIActive()
    {
        return (Attributes.gameObject.activeSelf || Inventory.gameObject.activeSelf || Daily.gameObject.activeSelf || Map.gameObject.activeSelf || isConversationActive) && !Pause.GameIsPaused;
    }

    void Update()
    {
        bool isAnyUIOpen = Attributes.gameObject.activeSelf || Inventory.gameObject.activeSelf || Daily.gameObject.activeSelf || Map.gameObject.activeSelf || isConversationActive;

        // Só permite abrir o inventário ou atributos se a conversa não estiver ativa
        if (!isConversationActive)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleAttributes();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                ToggleDaily();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleMap();
            }
        }
    }
    void ToggleAttributes()
    {
        if (Pause.GameIsPaused) return; // Bloqueia abertura se o jogo estiver pausado

        bool isActive = Attributes.gameObject.activeSelf;
        Attributes.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Inventory.gameObject.SetActive(false);
            Map.gameObject.SetActive(false);
            Daily.gameObject.SetActive(false);
        }
    }

    void ToggleInventory()
    {
        if (Pause.GameIsPaused) return; // Bloqueia abertura se o jogo estiver pausado

        bool isActive = Inventory.gameObject.activeSelf;
        Inventory.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Daily.gameObject.SetActive(false);
            Attributes.gameObject.SetActive(false);
            Map.gameObject.SetActive(false);
        }
    }

    void ToggleDaily()
    {
        if (Pause.GameIsPaused) return; // Bloqueia abertura se o jogo estiver pausado

        bool isActive = Daily.gameObject.activeSelf;
        Daily.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Inventory.gameObject.SetActive(false);
            Attributes.gameObject.SetActive(false);
            Map.gameObject.SetActive(false);
        }
    }

    void ToggleMap()
    {
        if (Pause.GameIsPaused) return; // Bloqueia abertura se o jogo estiver pausado

        bool isActive = Map.gameObject.activeSelf;
        Map.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Inventory.gameObject.SetActive(false);
            Attributes.gameObject.SetActive(false);
            Daily.gameObject.SetActive(false);
        }
    }

    public void StartConversation()
    {
        isConversationActive = true; // Marca a conversa como ativa
    }

    public void EndConversation()
    {
        isConversationActive = false;
    }
}
