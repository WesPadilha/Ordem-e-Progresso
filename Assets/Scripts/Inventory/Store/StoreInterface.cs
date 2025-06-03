using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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
                // Force immediate UI update
                vendorSlot.UpdateSlotUI();
            }
        }

        if (quantitySelector != null)
        {
            quantitySelector.SetStoreInterface(this);
            quantitySelector.selectorPanel.SetActive(false);
        }
        
        // Force update all slots after creation
        StartCoroutine(DelayedSlotUpdate());
    }

    private IEnumerator DelayedSlotUpdate()
    {
        yield return null; // Wait one frame
        foreach (var slot in slotsOnInterface.Values)
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
    
    public void UpdateAllSlotDisplays()
    {
        foreach (var slot in slotsOnInterface.Values)
        {
            UpdateSlotDisplay(slot);
        }
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)),
                         Y_START + (-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMN)), 0f);
    }

    protected new void OnDragEnd(GameObject obj)
    {
        if (!MouseData.isDragging || !Input.GetMouseButtonUp(0)) return;

        if(MouseData.tempItemBeingDragged != null)
        {
            Destroy(MouseData.tempItemBeingDragged);
        }
        
        if (MouseData.interfaceMouseIsOver == this)
        {
            MouseData.Reset();
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
        
        MouseData.Reset();
    }

    public void TransferItem(InventorySlot sourceSlot, InventorySlot targetSlot, int amount, bool isBuying)
    {
        if (sourceSlot.ItemObject == null || sourceSlot.amount <= 0) return;
        
        amount = Mathf.Min(amount, sourceSlot.amount);
        ItemObject itemObject = sourceSlot.ItemObject;
        int itemPrice = itemObject.price;
        
        // Aplica o bônus de negociação
        float priceMultiplier = isBuying ? 
            playerStatus.characterData.GetNegotiationBuyDiscount() : 
            playerStatus.characterData.GetNegotiationSellBonus();
        
        int totalPrice = Mathf.RoundToInt(amount * itemPrice * priceMultiplier);
        
        // Garante que o preço mínimo seja 1
        totalPrice = Mathf.Max(1, totalPrice);

        if (!targetSlot.CanPlaceInSlot(itemObject)) return;

        bool canStack = (targetSlot.ItemObject != null && 
                        targetSlot.ItemObject.data.Id == itemObject.data.Id && 
                        targetSlot.ItemObject.stackable) || 
                        targetSlot.ItemObject == null;

        if (!canStack) return;

        if (isBuying)
        {
            if (playerInventory.Money < totalPrice) return;
        }
        else
        {
            if (inventory.Money < totalPrice) return;
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

        InventoryWeightManager weightManager = FindObjectOfType<InventoryWeightManager>();
        if (weightManager != null)
        {
            weightManager.CalculateTotalWeight();
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