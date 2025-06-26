using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;
    public Attribute[] attributes;

    private Transform weapon;
    private Transform armor;
    public Transform weaponTransform;

    private Animator animator;
    private int currentWeaponLayerIndex = 0; // 0 = base, 1 = short, 2 = medium, 3 = long range
    //private float layerTransitionSpeed = 1f;

    private BoneCombiner boneCombiner;
    
    public CharacterData characterData;
    public UserInterface equipmentUI;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
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

        boneCombiner = new BoneCombiner(gameObject);

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
            if (attributes[i].type == AttributesItem.damage)
            {
                characterData.damage = attributes[i].value.BaseValue;
            }
            else if (attributes[i].type == AttributesItem.defense)
            {
                characterData.defense = attributes[i].value.BaseValue;
            }
        }
        
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            if (equipment.GetSlots[i].parent == null)
            {
                equipment.GetSlots[i].parent = equipmentUI;
            }
            
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
        
        foreach (var slot in equipment.GetSlots)
        {
            if (slot.ItemObject != null)
            {
                OnAddItem(slot);
            }
        }
    }

    private void RecalculateCharacterData()
    {
        int totalDamage = 0;
        int totalDefense = 0;

        foreach (var attribute in attributes)
        {
            if (attribute.type == AttributesItem.damage)
            {
                totalDamage = attribute.value.ModifiedValue; // Valor final com mods
            }
            else if (attribute.type == AttributesItem.defense)
            {
                totalDefense = attribute.value.ModifiedValue;
            }
        }

        characterData.damage = totalDamage;
        characterData.defense = totalDefense;
        characterData.NotifyChanges();
    }

    public void OnRemoveItem(InventorySlot _slot)
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
                            //UpdateCharacterData(_slot.item.buffs[i], false);
                        }
                    }
                }
                RecalculateCharacterData();

                if (_slot.ItemObject.characterDisplay != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        case ItemType.Weapon:
                            if (weapon != null)
                            {
                                Destroy(weapon.gameObject);
                            }
                            currentWeaponLayerIndex = 0;
                            ResetAllAnimationLayers();
                            break;
                        case ItemType.Armor:
                            if (armor != null)
                            {
                                Destroy(armor.gameObject);
                            }
                            break;
                    }
                }
                break;
        }
    }

    public void OnAddItem(InventorySlot _slot)
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
                            //UpdateCharacterData(_slot.item.buffs[i], true);
                        }
                    }
                }
                RecalculateCharacterData();

                if (_slot.ItemObject.characterDisplay != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        case ItemType.Weapon:
                            if (weapon != null)
                            {
                                Destroy(weapon.gameObject);
                            }
                            weapon = Instantiate(_slot.ItemObject.characterDisplay, weaponTransform).transform;
                            SetWeaponAnimationLayer(_slot.ItemObject.weaponRangeType);
                            break;
                        case ItemType.Armor:
                            if (armor != null)
                            {
                                Destroy(armor.gameObject);
                            }
                            armor = boneCombiner.AddLimb(_slot.ItemObject.characterDisplay, _slot.ItemObject.boneNames);
                            break;
                    }
                }
                break;
        }
    }

    private void SetWeaponAnimationLayer(WeaponRangeType weaponType)
    {
        currentWeaponLayerIndex = (int)weaponType;
        
        if (animator != null)
        {
            // Ativa imediatamente a camada correta
            for (int i = 1; i <= 3; i++)
            {
                animator.SetLayerWeight(i, i == currentWeaponLayerIndex ? 1f : 0f);
            }
        }
    }

    private void ResetAllAnimationLayers()
    {
        if (animator != null)
        {
            for (int i = 1; i <= 3; i++)
            {
                animator.SetLayerWeight(i, 0f);
            }
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
