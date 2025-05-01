using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StoreInterface : UserInterface
{
    public GameObject inventoryPrefab;
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEM;
    
    public QuantitySelectorUI quantitySelector;
    public InventoryObject playerInventory;
    public PlayerStatus playerStatus;
    public VendorUI vendorUI;

    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        
        for(int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            
            inventory.GetSlots[i].slotDisplay = obj;
            slotsOnInterface.Add(obj, inventory.GetSlots[i]);
            
            VendorSlot vendorSlot = obj.GetComponent<VendorSlot>();
            if (vendorSlot != null)
            {
                vendorSlot.SetupSlot(inventory.GetSlots[i]);
            }
        }

        if (quantitySelector != null)
        {
            quantitySelector.SetStoreInterface(this);
            quantitySelector.selectorPanel.SetActive(false);
        }
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), 
                         Y_START + (-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMN)), 0f);
    }

    protected new void OnDragStart(GameObject obj)
    {
        if (!slotsOnInterface.TryGetValue(obj, out InventorySlot slot) || slot.ItemObject == null)
            return;

        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }

    protected new void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    protected new void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        
        if (MouseData.interfaceMouseIsOver == this)
        {
            return;
        }

        if (MouseData.interfaceMouseIsOver is DynamicInterface && MouseData.slotHoveredOver)
        {
            InventorySlot playerSlot = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            InventorySlot vendorSlot = slotsOnInterface[obj];
            
            if (vendorSlot.ItemObject.stackable && vendorSlot.amount > 1)
            {
                quantitySelector.ShowSelector(vendorSlot, playerSlot, true);
            }
            else
            {
                TransferItem(vendorSlot, playerSlot, 1, true);
            }
        }
        else if (MouseData.interfaceMouseIsOver != null && MouseData.slotHoveredOver)
        {
            InventorySlot vendorSlot = slotsOnInterface[MouseData.slotHoveredOver];
            InventorySlot playerSlot = MouseData.interfaceMouseIsOver.slotsOnInterface[obj];
            
            if (playerSlot.ItemObject.stackable && playerSlot.amount > 1)
            {
                quantitySelector.ShowSelector(playerSlot, vendorSlot, false);
            }
            else
            {
                TransferItem(playerSlot, vendorSlot, 1, false);
            }
        }
    }

    protected new GameObject CreateTempItem(GameObject obj)
    {
        if (!slotsOnInterface.TryGetValue(obj, out InventorySlot slot) || slot.ItemObject == null)
            return null;

        GameObject tempItem = new GameObject();
        var rt = tempItem.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        tempItem.transform.SetParent(transform.parent);
        
        var img = tempItem.AddComponent<Image>();
        img.sprite = slot.ItemObject.uiDisplay;
        img.raycastTarget = false;
        
        return tempItem;
    }

    public void TransferItem(InventorySlot sourceSlot, InventorySlot targetSlot, int amount, bool isBuying)
    {
        if (sourceSlot.ItemObject == null || sourceSlot.amount <= 0) 
        {
            Debug.LogWarning("Nenhum item para transferir!");
            return;
        }
        
        amount = Mathf.Min(amount, sourceSlot.amount);
        ItemObject itemObject = sourceSlot.ItemObject;
        int itemPrice = itemObject.price;
        int totalPrice = amount * itemPrice;

        if (!targetSlot.CanPlaceInSlot(itemObject)) 
        {
            Debug.Log("Não pode colocar neste slot!");
            return;
        }

        bool canStack = (targetSlot.ItemObject != null && 
                        targetSlot.ItemObject.data.Id == itemObject.data.Id && 
                        targetSlot.ItemObject.stackable) || 
                        targetSlot.ItemObject == null;

        if (!canStack)
        {
            Debug.Log("Itens não são compatíveis para stack!");
            return;
        }

        if (isBuying)
        {
            if (playerInventory.Money < totalPrice)
            {
                Debug.Log("Dinheiro insuficiente!");
                return;
            }
        }
        else
        {
            if (inventory.Money < totalPrice)
            {
                Debug.Log("O vendedor não tem dinheiro suficiente!");
                return;
            }
        }

        int newSourceAmount = sourceSlot.amount - amount;
        if (newSourceAmount <= 0)
        {
            sourceSlot.RemoveItem();
        }
        else
        {
            sourceSlot.UpdateSlot(sourceSlot.item, newSourceAmount);
        }
        
        Item itemToTransfer = new Item(itemObject);
        if (targetSlot.ItemObject == null)
        {
            targetSlot.UpdateSlot(itemToTransfer, amount);
        }
        else
        {
            targetSlot.AddAmount(amount);
        }
        
        if (isBuying)
        {
            playerInventory.Money -= totalPrice;
            inventory.Money += totalPrice;
        }
        else
        {
            playerInventory.Money += totalPrice;
            inventory.Money -= totalPrice;
        }
        
        playerStatus.UpdateMoneyDisplay();
        vendorUI.UpdateVendorMoneyDisplay();

        UpdateSlotDisplay(sourceSlot);
        UpdateSlotDisplay(targetSlot);
    }

    private void UpdateSlotDisplay(InventorySlot slot)
    {
        if (slot.slotDisplay != null)
        {
            VendorSlot vendorSlot = slot.slotDisplay.GetComponent<VendorSlot>();
            if (vendorSlot != null) 
            {
                vendorSlot.UpdateSlotUI();
            }
        }
    }
}