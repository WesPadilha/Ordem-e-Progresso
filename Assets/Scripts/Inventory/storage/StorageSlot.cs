using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StorageSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    private ItemObject item;
    private int quantity = 0;

    private StorageUI storageUI;

    void Start()
    {
        storageUI = GetComponentInParent<StorageUI>();
    }

    public void SetItem(ItemObject newItem, int amount = 1)
    {
        if (item != null && item == newItem && item.stackable)
        {
            quantity += amount;
        }
        else
        {
            item = newItem;
            quantity = amount;
        }

        if (item != null && item.uiDisplay != null)
        {
            icon.sprite = item.uiDisplay;
            icon.enabled = true;
        }

        UpdateQuantityText();
    }

    public void ClearSlot(Sprite defaultIcon)
    {
        item = null;
        quantity = 0;
        icon.sprite = defaultIcon;
        icon.enabled = true;
        UpdateQuantityText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null)
        {
            if (item.stackable)
            {
                storageUI.TransferItemToInventory(item, quantity); // Transfere tudo
                ClearSlot(storageUI.defaultIcon);
            }
            else
            {
                storageUI.TransferItemToInventory(item, 1);
                quantity--;
                if (quantity <= 0)
                    ClearSlot(storageUI.defaultIcon);
                else
                    UpdateQuantityText();
            }
        }
    }

    public ItemObject GetItem() => item;
    public int GetQuantity() => quantity;

    private void UpdateQuantityText()
    {
        quantityText.text = (item != null && item.stackable && quantity > 1) ? quantity.ToString() : "";
    }
}
