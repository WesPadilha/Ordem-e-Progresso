using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;
    public Attribute[] attributes;
    public CharacterData characterData;
    public UserInterface equipmentUI; // Add reference to equipment UI

    private void Start()
    {
        // Validate references
        if (equipment == null)
        {
            Debug.LogError("Equipment inventory not assigned in Collect script");
            return;
        }

        if (equipment.database == null)
        {
            Debug.LogError("Database not assigned in Equipment inventory");
            return;
        }

        // Initialize attributes
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
            // Initialize CharacterData with base attribute values
            if (attributes[i].type == AttributesItem.damage)
            {
                characterData.damage = attributes[i].value.BaseValue;
            }
            else if (attributes[i].type == AttributesItem.defense)
            {
                characterData.defense = attributes[i].value.BaseValue;
            }
        }
        
        // Initialize equipment slots
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            // Set parent reference if not set
            if (equipment.GetSlots[i].parent == null)
            {
                equipment.GetSlots[i].parent = equipmentUI;
            }
            
            equipment.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
            equipment.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
        }
        
        // Initialize with any currently equipped items
        foreach (var slot in equipment.GetSlots)
        {
            if (slot.ItemObject != null)
            {
                OnAfterSlotUpdate(slot);
            }
        }
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
            
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Equipment:
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attributesItem)
                        {
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                            UpdateCharacterData(_slot.item.buffs[i], false);
                        }
                    }
                }
                break;
        }
    }

    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
            
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Equipment:
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attributesItem)
                        {
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                            UpdateCharacterData(_slot.item.buffs[i], true);
                        }
                    }
                }
                break;
        }
    }

    private void UpdateCharacterData(ItemBuff buff, bool isAdding)
    {
        if (characterData == null) return;

        int modifier = isAdding ? buff.value : -buff.value;

        switch (buff.attributesItem)
        {
            case AttributesItem.damage:
                characterData.damage += modifier;
                break;
            case AttributesItem.defense:
                characterData.defense += modifier;
                break;
        }

        characterData.NotifyChanges();
    }

    public void OnTriggerEnter(Collider other)
    {
        var groundItem = other.GetComponent<GroundItem>();
        if (groundItem)
        {
            Item _item = new Item(groundItem.item);
            if (inventory.AddItem(_item, 1))
            {
                Destroy(other.gameObject);
            }
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
        
        // Update CharacterData when attributes are modified
        if (characterData != null)
        {
            if (attribute.type == AttributesItem.damage)
            {
                characterData.damage = attribute.value.ModifiedValue;
            }
            else if (attribute.type == AttributesItem.defense)
            {
                characterData.defense = attribute.value.ModifiedValue;
            }
            characterData.NotifyChanges();
        }
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public Collect parent;
    public AttributesItem type;
    public ModifiableInt value;

    public void SetParent(Collect _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}