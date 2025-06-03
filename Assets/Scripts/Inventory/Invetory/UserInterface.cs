using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class UserInterface : MonoBehaviour
{
    public InventoryObject inventory;
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    protected virtual void Start()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }
        CreateSlots();

        slotsOnInterface.UpdateSlotDisplay();

        // Atualiza o peso do inventário ao inicializar a interface
        var weightManager = FindObjectOfType<InventoryWeightManager>();
        if (weightManager != null)
        {
            weightManager.CalculateTotalWeight();
        }
        
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    private void OnSlotUpdate(InventorySlot _slot)
    {
        // Check if the slot display still exists
        if (_slot.slotDisplay == null)
            return;

        // Check if the slot display has been destroyed
        if (_slot.slotDisplay.gameObject == null)
            return;

        // Get the transform safely
        Transform slotTransform = _slot.slotDisplay.transform;
        if (slotTransform == null || slotTransform.childCount == 0)
            return;

        Transform child = slotTransform.GetChild(0);
        if (child == null)
            return;

        Image image = child.GetComponentInChildren<Image>();
        TextMeshProUGUI text = _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>();

        if (_slot.item.Id >= 0)
        {
            if (image != null)
            {
                image.sprite = _slot.ItemObject.uiDisplay;
                image.color = new Color(1, 1, 1, 1);
            }

            if (text != null)
            {
                text.text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
            }
        }
        else
        {
            if (image != null)
            {
                image.sprite = null;
                image.color = new Color(1, 1, 1, 0);
            }

            if (text != null)
            {
                text.text = "";
            }
        }
    }

    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
    }

    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }

    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    public void OnDragStart(GameObject obj)
    {
        if (!Input.GetMouseButton(0)) return;
        if (MouseData.isDragging) return;

        MouseData.isDragging = true;
        MouseData.tempItemBeingDragged = CreateTempItem(obj);

        if (MouseData.tempItemBeingDragged != null)
        {
            MouseData.tempItemBeingDragged.transform.position = Input.mousePosition;
        }
    }

    public GameObject CreateTempItem(GameObject obj)
    {
        if (slotsOnInterface[obj].item.Id < 0) return null;

        if (MouseData.tempItemBeingDragged != null)
        {
            Destroy(MouseData.tempItemBeingDragged);
        }

        GameObject tempItem = new GameObject("TempDraggedItem");
        var rt = tempItem.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
        tempItem.transform.SetParent(transform.parent.parent);

        var img = tempItem.AddComponent<Image>();
        img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
        img.raycastTarget = false;

        var canvasGroup = tempItem.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        return tempItem;
    }

    public void OnDragEnd(GameObject obj)
    {
        if (!MouseData.isDragging || !Input.GetMouseButtonUp(0)) return;

        if (MouseData.tempItemBeingDragged != null)
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
            // Verifica se está tentando mover para o mesmo slot
            if (MouseData.slotHoveredOver == obj)
            {
                MouseData.Reset();
                return;
            }

            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }

        MouseData.Reset();
    }

    public void OnDrag(GameObject obj)
    {
        if (!MouseData.isDragging || !Input.GetMouseButton(0)) return;

        if (MouseData.tempItemBeingDragged != null)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = MouseData.tempItemBeingDragged.transform.position.z;
            MouseData.tempItemBeingDragged.transform.position = pos;
        }
    }
    private void OnDestroy()
    {
        // Unsubscribe from all slot events
        if (inventory != null)
        {
            foreach (var slot in inventory.GetSlots)
            {
                if (slot != null)
                {
                    slot.OnAfterUpdate -= OnSlotUpdate;
                }
            }
        }
    }
}
public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
    public static bool isDragging = false;
    
    public static void Reset()
    {
        if(tempItemBeingDragged != null)
        {
            GameObject.Destroy(tempItemBeingDragged);
        }
        tempItemBeingDragged = null;
        slotHoveredOver = null;
        isDragging = false;
    }
}

public static class ExtensionMethods
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        foreach(KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
        {
            if(_slot.Value.item.Id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.uiDisplay;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else    
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}