using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject Attributes;
    public MainCameraController mainCameraController;   
    private bool isConversationActive = false; // Acompanhar se a conversa está ativa

    public bool IsAnyUIActive()
    {
        return Attributes.gameObject.activeSelf || Inventory.gameObject.activeSelf || isConversationActive;
    }

    void Update()
    {
        bool isAnyUIOpen = Attributes.gameObject.activeSelf || Inventory.gameObject.activeSelf || isConversationActive;

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
        }

        // Controle de ESC para fechar a conversa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isConversationActive)
            {
                EndConversation(); // Fecha a conversa ao apertar ESC
            }
        }
    }

    void ToggleAttributes()
    {
        bool isActive = Attributes.gameObject.activeSelf;
        Attributes.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Inventory.gameObject.SetActive(false);
        }
    }

    void ToggleInventory()
    {
        bool isActive = Inventory.gameObject.activeSelf;
        Inventory.gameObject.SetActive(!isActive);
        if (!isActive)
        {
            Attributes.gameObject.SetActive(false);
        }
    }

    public void StartConversation()
    {
        isConversationActive = true; // Marca a conversa como ativa
    }

    public void EndConversation()
    {
        isConversationActive = false; // Marca a conversa como inativa
        // Finaliza a conversa no ConversationManager (se necessário)
        DialogueEditor.ConversationManager.Instance.EndConversation();
    }
}
