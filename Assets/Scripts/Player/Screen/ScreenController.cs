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
    private bool isStoreOpen = false; // Estado da loja

    public bool IsAnyUIActive()
    {
        return (Attributes.gameObject.activeSelf || Inventory.gameObject.activeSelf || Daily.gameObject.activeSelf || Map.gameObject.activeSelf || isConversationActive) && !Pause.GameIsPaused;
    }

    void Update()
    {
        bool isAnyUIOpen = Attributes.gameObject.activeSelf || Inventory.gameObject.activeSelf || Daily.gameObject.activeSelf || Map.gameObject.activeSelf || isConversationActive;

        // Fecha tudo ao pressionar Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllUI();
        }

        // Só permite abrir o inventário ou atributos se a conversa não estiver ativa e a loja não estiver aberta
        if (!isConversationActive && !isStoreOpen)
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

    public void ToggleAttributes()
    {
        if (Pause.GameIsPaused || isStoreOpen) return; // Bloqueia se o jogo estiver pausado ou loja aberta

        bool isActive = Attributes.gameObject.activeSelf;
        Attributes.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Inventory.gameObject.SetActive(false);
            Map.gameObject.SetActive(false);
            Daily.gameObject.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        if (Pause.GameIsPaused || isStoreOpen) return; // Bloqueia se o jogo estiver pausado ou loja aberta

        bool isActive = Inventory.gameObject.activeSelf;
        Inventory.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Daily.gameObject.SetActive(false);
            Attributes.gameObject.SetActive(false);
            Map.gameObject.SetActive(false);
        }
    }

    public void ToggleDaily()
    {
        if (Pause.GameIsPaused || isStoreOpen) return; // Bloqueia se o jogo estiver pausado ou loja aberta

        bool isActive = Daily.gameObject.activeSelf;
        Daily.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Inventory.gameObject.SetActive(false);
            Attributes.gameObject.SetActive(false);
            Map.gameObject.SetActive(false);
        }
    }

    public void ToggleMap()
    {
        if (Pause.GameIsPaused || isStoreOpen) return; // Bloqueia se o jogo estiver pausado ou loja aberta

        bool isActive = Map.gameObject.activeSelf;
        Map.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Inventory.gameObject.SetActive(false);
            Attributes.gameObject.SetActive(false);
            Daily.gameObject.SetActive(false);
        }
    }

    public void CloseAllUI()
    {
        Attributes.gameObject.SetActive(false);
        Inventory.gameObject.SetActive(false);
        Daily.gameObject.SetActive(false);
        Map.gameObject.SetActive(false);

        if (isStoreOpen)
        {
            FindObjectOfType<StoreScreen>().CloseStore();
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

    public void SetStoreState(bool state)
    {
        isStoreOpen = state;
    }
}