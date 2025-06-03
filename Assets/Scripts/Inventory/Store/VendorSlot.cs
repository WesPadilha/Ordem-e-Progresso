using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class VendorSlot : MonoBehaviour
{
    [Header("UI Components")]
    public Image icon;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI amountText;
    
    [System.NonSerialized] public InventorySlot inventorySlot;
    private PlayerStatus playerStatus;
    private bool isDestroyed = false;

    private void Awake()
    {
        if (icon == null) icon = GetComponent<Image>();
        if (priceText == null && transform.Find("PriceText") != null) 
            priceText = transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        if (amountText == null && transform.Find("AmountText") != null) 
            amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
        
        playerStatus = FindObjectOfType<PlayerStatus>();
        if(playerStatus != null && playerStatus.characterData != null)
        {
            playerStatus.characterData.OnDataChanged += UpdateSlotUI;
        }
    }

    public void SetupSlot(InventorySlot slot)
    {
        if (isDestroyed) return;

        // Unsubscribe from previous slot if exists
        if (this.inventorySlot != null)
        {
            this.inventorySlot.OnAfterUpdate -= OnSlotUpdated;
        }

        this.inventorySlot = slot;
        if (slot != null)
        {
            slot.OnAfterUpdate += OnSlotUpdated;
            // Force immediate update
            UpdateSlotUI();
            
            // Additional check for first-time setup
            if (priceText != null && priceText.text == "")
            {
                UpdateSlotUI();
            }
        }
        else
        {
            ClearSlot();
        }
    }

    private void OnSlotUpdated(InventorySlot slot)
    {
        if (!isDestroyed)
        {
            UpdateSlotUI();
        }
    }

    public void UpdateSlotUI()
    {
        if (isDestroyed || this == null) return;

        if (inventorySlot == null || inventorySlot.ItemObject == null)
        {
            ClearSlot();
            return;
        }

        if (icon != null)
        {
            icon.sprite = inventorySlot.ItemObject.uiDisplay;
            icon.color = Color.white;
        }
        
        bool isPlayerItem = inventorySlot.parent != null && 
                          inventorySlot.parent.inventory != null && 
                          inventorySlot.parent.inventory.type == InterfaceType.Inventory;
        int basePrice = inventorySlot.ItemObject.price;
        
        if (priceText != null)
        {
            if (playerStatus != null && playerStatus.characterData != null)
            {
                float priceMultiplier = isPlayerItem ? 
                    playerStatus.characterData.GetNegotiationSellBonus() : 
                    playerStatus.characterData.GetNegotiationBuyDiscount();
                
                int adjustedPrice = Mathf.RoundToInt(basePrice * priceMultiplier);
                priceText.text = "$" + adjustedPrice;
            }
            else
            {
                priceText.text = "$" + basePrice;
            }
        }

        if (amountText != null)
        {
            if (inventorySlot.ItemObject.stackable && inventorySlot.amount > 1)
            {
                amountText.text = inventorySlot.amount.ToString("n0");
                amountText.gameObject.SetActive(true);
            }
            else
            {
                amountText.gameObject.SetActive(false);
            }
        }
    }

    private void ClearSlot()
    {
        if (isDestroyed || this == null) return;

        if (icon != null)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }
        
        if (priceText != null)
        {
            priceText.text = "";
        }
        
        if (amountText != null)
        {
            amountText.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        isDestroyed = true;
        
        if (inventorySlot != null)
        {
            inventorySlot.OnAfterUpdate -= OnSlotUpdated;
        }
        
        if(playerStatus != null && playerStatus.characterData != null)
        {
            playerStatus.characterData.OnDataChanged -= UpdateSlotUI;
        }
    }
}