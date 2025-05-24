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

    private void OnUseClicked()
    {
        if (currentSlot == null || currentSlot.ItemObject == null) 
            return;

        if (currentSlot.ItemObject.type == ItemType.consumables)
        {
            UseConsumableItem();
        }
        else if (currentSlot.ItemObject.type == ItemType.Weapon || 
                currentSlot.ItemObject.type == ItemType.Armor)
        {
            EquipItem();
        }
        else
        {
            Debug.Log($"Este item não pode ser usado diretamente: {currentSlot.ItemObject.name}");
        }
        
        CloseAllPanels();
    }

    private void EquipItem()
    {
        // Encontra a interface de equipamento (assumindo que você tem apenas uma)
        StaticInterface equipmentUI = FindObjectOfType<StaticInterface>();
        if (equipmentUI == null)
        {
            Debug.LogWarning("Interface de equipamento não encontrada!");
            return;
        }

        // Determina o tipo de slot necessário
        ItemType itemType = currentSlot.ItemObject.type;
        InventorySlot targetSlot = FindAppropriateEquipmentSlot(equipmentUI, itemType);

        if (targetSlot == null)
        {
            Debug.LogWarning($"Nenhum slot de equipamento adequado encontrado para {itemType}");
            return;
        }

        // Move o item para o slot de equipamento
        if (targetSlot.ItemObject == null) // Slot vazio
        {
            targetSlot.UpdateSlot(currentSlot.item, currentSlot.amount);
            currentSlot.RemoveItem();
        }
        else // Slot ocupado - faz a troca
        {
            Item tempItem = targetSlot.item;
            int tempAmount = targetSlot.amount;
            
            targetSlot.UpdateSlot(currentSlot.item, currentSlot.amount);
            currentSlot.UpdateSlot(tempItem, tempAmount);
        }
    }

    private InventorySlot FindAppropriateEquipmentSlot(StaticInterface equipmentUI, ItemType itemType)
    {
        foreach (var slot in equipmentUI.slotsOnInterface.Values)
        {
            // Verifica se o slot permite este tipo de item
            if (System.Array.Exists(slot.AllowedItems, allowed => allowed == itemType))
            {
                return slot;
            }
        }
        return null;
    }

    private void UseConsumableItem()
    {
        PlayerLife playerLife = FindObjectOfType<PlayerLife>();
        if (playerLife == null)
        {
            Debug.LogWarning("PlayerLife não encontrado!");
            return;
        }

        // Guarda uma referência ao item antes de remover
        ItemObject itemObject = currentSlot.ItemObject;
        int amountBefore = currentSlot.amount;

        // Aplica os buffs
        foreach (var buff in currentSlot.item.buffs)
        {
            if (buff.attributesItem == AttributesItem.heal)
            {
                playerLife.Heal(buff.value);
            }
        }

        // Remove uma unidade do item
        currentSlot.AddAmount(-1);

        // Notifica a mudança de peso
        if (currentSlot.parent != null && currentSlot.parent.inventory != null)
        {
            // Se o item foi completamente removido
            if (currentSlot.amount <= 0)
            {
                currentSlot.parent.inventory.NotifyItemRemoved(itemObject, 1);
                currentSlot.RemoveItem();
            }
            else if (amountBefore != currentSlot.amount) // Se a quantidade mudou
            {
                currentSlot.parent.inventory.NotifyItemRemoved(itemObject, 1);
            }
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