using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DynamicInterface : UserInterface
{
    public GameObject inventoryPrefab;
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEM;

    public QuantitySelectorUI quantitySelector;
    
    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
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
        
        if (MouseData.interfaceMouseIsOver == null)
        {
            FindObjectOfType<DiscardConfirmationUI>().AskForConfirmation(slotsOnInterface[obj]);
            MouseData.Reset();
            return;
        }

        if (MouseData.slotHoveredOver)
        {
            if (MouseData.interfaceMouseIsOver is StoreInterface storeInterface)
            {
                InventorySlot playerSlot = slotsOnInterface[obj];
                InventorySlot vendorSlot = storeInterface.slotsOnInterface[MouseData.slotHoveredOver];
                
                if (playerSlot.ItemObject.stackable && playerSlot.amount > 1)
                {
                    storeInterface.quantitySelector.ShowSelector(playerSlot, vendorSlot, false);
                }
                else
                {
                    storeInterface.TransferItem(playerSlot, vendorSlot, 1, false);
                }
            }
            else
            {
                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
                inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
            }
        }
        
        MouseData.Reset();
    }
}