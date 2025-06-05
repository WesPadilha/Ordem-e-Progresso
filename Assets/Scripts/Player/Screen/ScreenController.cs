using UnityEngine;
using DialogueEditor;

public class ScreenController : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject Attributes;
    public GameObject Daily;
    public GameObject Map;
    public MainCameraController mainCameraController;   
    private bool isConversationActive = false;
    private bool isStoreOpen = false;
    private bool isStorageOpen = false;
    private ItemContextMenu itemContextMenu;
    private TurnManager turnManager;
    private MovimentCombat playerMovement;
    private CombatStatusChecker combatStatusChecker;

    private void Awake()
    {
        itemContextMenu = FindObjectOfType<ItemContextMenu>();
        turnManager = FindObjectOfType<TurnManager>();
        playerMovement = FindObjectOfType<MovimentCombat>();
        combatStatusChecker = FindObjectOfType<CombatStatusChecker>();
    }

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

        // Verifica se está em combate e não é o turno do jogador
        bool isEnemyTurn = combatStatusChecker.IsInCombat() && turnManager.currentPhase == TurnManager.TurnPhase.Enemies;
        
        // Só permite abrir interfaces se não for turno dos inimigos
        if (!isConversationActive && !isStoreOpen && !isStorageOpen && !isEnemyTurn)
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
        if (Pause.GameIsPaused || isStoreOpen || isStorageOpen) return;

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
        if (Pause.GameIsPaused || isStoreOpen || isStorageOpen) return;

        bool isOpening = !Inventory.gameObject.activeSelf;

        // Se estiver em combate e for abrir o inventário (não fechar)
        if (isOpening && combatStatusChecker.IsInCombat())
        {
            if (turnManager.currentPhase != TurnManager.TurnPhase.Player)
            {
                Debug.Log("Não é turno do jogador para abrir o inventário!");
                return;
            }

            if (playerMovement.GetCurrentActionPoints() < 4)
            {
                Debug.Log("Pontos de ação insuficientes para abrir o inventário em combate!");
                return;
            }

            // Gasta apenas 4 pontos de ação ao abrir o inventário
            playerMovement.SpendActionPoints(4);
        }

        bool isActive = Inventory.gameObject.activeSelf;
        Inventory.gameObject.SetActive(!isActive);
        
        if (!isActive && itemContextMenu != null)
        {
            itemContextMenu.CloseContextMenu();
        }
        
        if (!isActive)
        {
            Daily.gameObject.SetActive(false);
            Attributes.gameObject.SetActive(false);
            Map.gameObject.SetActive(false);
        }
    }

    public void ToggleDaily()
    {
        if (Pause.GameIsPaused || isStoreOpen || isStorageOpen) return;

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
        if (Pause.GameIsPaused || isStoreOpen || isStorageOpen) return;

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

        // Fecha o menu de contexto se estiver aberto
        if (itemContextMenu != null)
        {
            itemContextMenu.CloseContextMenu();
        }

        if (isStoreOpen)
        {
            FindObjectOfType<StoreScreen>().CloseStore();
        }
        if (isStorageOpen)
        {
            FindObjectOfType<Storage>().CloseStorage();
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

    public void SetStorageState(bool state)
    {
        isStorageOpen = state;
    }
    public bool IsStorageOpen()
    {
        return isStorageOpen; // Retorna o estado do armazenamento (se está aberto ou fechado)
    }
    public bool IsStoreOpen()
    {
        return isStoreOpen;
    }
}
