using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemContextMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject contextMenuPanel;
    public Button useButton;
    public Button splitButton;
    public Button descriptionButton;
    public TextMeshProUGUI descriptionText;
    public GameObject descriptionPanel;
    public Slider splitSlider;
    public TextMeshProUGUI splitAmountText;
    public Button splitConfirmButton;
    public Button splitCancelButton;

    private InventorySlot currentSlot;
    private InventoryObject inventory;
    private bool isSplitting = false;
    private ScreenController screenController;

    private void Awake()
    {
        screenController = FindObjectOfType<ScreenController>();
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Esconder todos os painéis no início
        if (contextMenuPanel != null) 
            contextMenuPanel.SetActive(false);
        
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
        
        if (splitSlider != null)
            splitSlider.gameObject.SetActive(false);
        
        if (splitConfirmButton != null)
            splitConfirmButton.gameObject.SetActive(false);
        
        if (splitCancelButton != null)
            splitCancelButton.gameObject.SetActive(false);

        // Esconder o texto de divisão inicialmente
        if (splitAmountText != null)
            splitAmountText.gameObject.SetActive(false);

        // Configurar listeners dos botões
        if (useButton != null)
            useButton.onClick.AddListener(OnUseClicked);
        
        if (splitButton != null)
            splitButton.onClick.AddListener(OnSplitClicked);
        
        if (descriptionButton != null)
            descriptionButton.onClick.AddListener(OnDescriptionClicked);
        
        if (splitConfirmButton != null)
            splitConfirmButton.onClick.AddListener(OnSplitConfirm);
        
        if (splitCancelButton != null)
            splitCancelButton.onClick.AddListener(OnSplitCancel);

        // Configurar slider
        if (splitSlider != null)
            splitSlider.onValueChanged.AddListener(UpdateSplitAmountText);
    }

    public void Initialize(InventoryObject inventory)
    {
        this.inventory = inventory;
    }

    public void ShowContextMenu(InventorySlot slot)
    {
        // Verificar se o inventário está aberto antes de mostrar o menu
        if (slot.ItemObject == null || !screenController.Inventory.activeSelf) 
            return;

        currentSlot = slot;
        
        // Ativar painel principal
        contextMenuPanel.SetActive(true);
        
        // Configurar botões baseados no tipo de item
        splitButton.gameObject.SetActive(slot.ItemObject.stackable && slot.amount > 1);
        
        // Esconder sub-painéis
        descriptionPanel.SetActive(false);
        isSplitting = false;
    }

    // No script ItemContextMenu, adicione este método:
    private void OnUseClicked()
    {
        if (currentSlot == null || currentSlot.ItemObject == null) 
            return;

        // Verifica se é um item consumível
        if (currentSlot.ItemObject.type == ItemType.consumables)
        {
            UseConsumableItem();
        }
        else
        {
            Debug.Log($"Item não é consumível: {currentSlot.ItemObject.name}");
        }
        
        CloseAllPanels();
    }

    private void UseConsumableItem()
    {
        // Obtém o componente PlayerLife (assumindo que está no mesmo GameObject ou você pode usar FindObjectOfType)
        PlayerLife playerLife = FindObjectOfType<PlayerLife>();
        if (playerLife == null)
        {
            Debug.LogWarning("PlayerLife não encontrado!");
            return;
        }

        // Aplica todos os buffs do item
        foreach (var buff in currentSlot.item.buffs)
        {
            if (buff.attributesItem == AttributesItem.heal)
            {
                playerLife.Heal(buff.value);
                Debug.Log($"Curando {buff.value} pontos de vida");
            }
            // Você pode adicionar outros tipos de buffs aqui se necessário
        }

        // Remove uma unidade do item do inventário
        currentSlot.AddAmount(-1);

        // Se a quantidade chegar a zero, remove o item
        if (currentSlot.amount <= 0)
        {
            currentSlot.RemoveItem();
        }
    }

    private void OnSplitClicked()
    {
        if (currentSlot == null || !currentSlot.ItemObject.stackable || currentSlot.amount <= 1) 
            return;

        // Configurar slider de divisão
        splitSlider.minValue = 1;
        splitSlider.maxValue = currentSlot.amount - 1;
        splitSlider.value = Mathf.Clamp(currentSlot.amount / 2, 1, currentSlot.amount - 1);

        // Mostrar UI de divisão
        splitSlider.gameObject.SetActive(true);
        splitAmountText.gameObject.SetActive(true); // Mostrar o texto de divisão
        splitConfirmButton.gameObject.SetActive(true);
        splitCancelButton.gameObject.SetActive(true);
        
        UpdateSplitAmountText(splitSlider.value);
        isSplitting = true;
    }

    private void OnSplitConfirm()
    {
        if (!isSplitting || currentSlot == null) return;
        
        int amountToSplit = (int)splitSlider.value;
        if (SplitStack(currentSlot, amountToSplit))
        {
            CloseAllPanels();
        }
    }

    private void OnSplitCancel()
    {
        isSplitting = false;
        splitSlider.gameObject.SetActive(false);
        splitAmountText.gameObject.SetActive(false); // Esconder o texto de divisão
        splitConfirmButton.gameObject.SetActive(false);
        splitCancelButton.gameObject.SetActive(false);
    }

    private void OnDescriptionClicked()
    {
        if (currentSlot == null || currentSlot.ItemObject == null) return;

        descriptionText.text = currentSlot.ItemObject.description;
        descriptionPanel.SetActive(true);
    }

    private void UpdateSplitAmountText(float value)
    {
        if (currentSlot == null) return;
        
        int amount = (int)value;
        splitAmountText.text = $"Dividir: {amount}/{currentSlot.amount}";
    }

    private bool SplitStack(InventorySlot slot, int amountToSplit)
    {
        if (slot == null || slot.ItemObject == null || slot.amount <= amountToSplit)
        {
            Debug.LogWarning("Não é possível dividir - slot ou item inválido");
            return false;
        }

        // Encontrar um slot vazio no mesmo inventário
        InventorySlot emptySlot = FindFirstEmptySlot(slot.parent.inventory);
        
        if (emptySlot == null)
        {
            Debug.Log("Inventário cheio - não é possível dividir o item");
            return false;
        }

        // Criar cópia do item para o novo slot
        Item itemCopy = new Item(slot.ItemObject);
        
        // Atualizar slots
        emptySlot.UpdateSlot(itemCopy, amountToSplit);
        slot.UpdateSlot(slot.item, slot.amount - amountToSplit);
        
        Debug.Log($"Item dividido: {amountToSplit} unidades movidas para novo slot");
        return true;
    }

    private InventorySlot FindFirstEmptySlot(InventoryObject inventory)
    {
        if (inventory == null) return null;
        
        foreach (var slot in inventory.GetSlots)
        {
            if (slot.item.Id <= -1) // Slot vazio
            {
                return slot;
            }
        }
        return null;
    }

    public void CloseContextMenu()
    {
        CloseAllPanels();
    }

    private void CloseAllPanels()
    {
        if (contextMenuPanel != null)
            contextMenuPanel.SetActive(false);
        
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
        
        if (splitSlider != null)
            splitSlider.gameObject.SetActive(false);
        
        if (splitAmountText != null)
            splitAmountText.gameObject.SetActive(false); // Esconder o texto de divisão
        
        if (splitConfirmButton != null)
            splitConfirmButton.gameObject.SetActive(false);
        
        if (splitCancelButton != null)
            splitCancelButton.gameObject.SetActive(false);
        
        isSplitting = false;
    }

    private void Update()
    {
        if (contextMenuPanel != null && contextMenuPanel.activeSelf && 
            (Input.GetMouseButtonDown(0) && !IsPointerOverContextMenu() || 
            (screenController != null && !screenController.Inventory.activeSelf)))
        {
            CloseAllPanels();
        }
    }

    private bool IsPointerOverContextMenu()
    {
        if (EventSystem.current == null) return false;
        
        // Cria um PointerEventData e verifica a posição do ponteiro
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        
        // Lista para armazenar resultados do raycast
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        // Verifica se algum dos resultados é parte do menu de contexto
        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(contextMenuPanel.transform) || 
                result.gameObject == contextMenuPanel)
            {
                return true;
            }
        }
        
        return false;
    }
}