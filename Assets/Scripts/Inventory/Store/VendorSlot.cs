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

    private void Awake()
    {
        if (icon == null) icon = GetComponent<Image>();
        if (priceText == null) priceText = transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>();
        if (amountText == null) amountText = transform.Find("AmountText")?.GetComponent<TextMeshProUGUI>();
    }

    public void SetupSlot(InventorySlot slot)
    {
        if (this.inventorySlot != null)
        {
            this.inventorySlot.OnAfterUpdate -= OnSlotUpdated;
        }

        this.inventorySlot = slot;
        slot.OnAfterUpdate += OnSlotUpdated;
        UpdateSlotUI();
    }

    private void OnSlotUpdated(InventorySlot slot)
    {
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (inventorySlot == null || inventorySlot.ItemObject == null)
        {
            ClearSlot();
            return;
        }

        icon.sprite = inventorySlot.ItemObject.uiDisplay;
        icon.color = Color.white;
        
        bool isPlayerItem = inventorySlot.parent.inventory.type == InterfaceType.Inventory;
        priceText.color = isPlayerItem ? Color.red : Color.green;
        priceText.text = "$" + inventorySlot.ItemObject.price;

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

    private void ClearSlot()
    {
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0);
        priceText.text = "";
        if (amountText != null) amountText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (inventorySlot != null)
        {
            inventorySlot.OnAfterUpdate -= OnSlotUpdated;
        }
    }
}