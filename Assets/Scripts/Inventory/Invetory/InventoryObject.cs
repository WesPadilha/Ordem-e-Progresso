using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;
public enum InterfaceType
{
    Inventory,
    Equipment,
    Chest,
    Store
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "InventorySystem/Inventory")]
public class InventoryObject : ScriptableObject
{
    public int Money;
    public string savePath;
    public ItemDatabaseObject database;
    public InterfaceType type;
    public Inventory Container;
    public InventorySlot[] GetSlots { get { return Container.Slots; } }

    public delegate void ItemEvent(ItemObject item, int amount);
    public event ItemEvent OnItemAdded;
    public event ItemEvent OnItemRemoved;

    public bool AddItem(Item _item, int _amount)
    {
        bool added = false;
        
        if(EmptySlotCount <= 0)
            return false;
            
        InventorySlot slot = FindItemOnInventory(_item);
        if(!database.ItemObjects[_item.Id].stackable || slot == null)
        {
            SetEmptySlot(_item, _amount);
            added = true;
        }
        else
        {
            slot.AddAmount(_amount);
            added = true;
        }

        if(added)
        {
            OnItemAdded?.Invoke(database.ItemObjects[_item.Id], _amount);
        }
        return added;
    }

    public void NotifyItemRemoved(ItemObject item, int amount)
    {
        OnItemRemoved?.Invoke(item, amount);
    }


    // Em InventoryObject.cs, adicione este método:
    public InventorySlot FindEmptySlot()
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.Id <= -1)
            {
                return GetSlots[i];
            }
        }
        return null;
    }
    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for(int i = 0; i < GetSlots.Length; i++)
            {
                if(GetSlots[i].item.Id <= -1)
                    counter++;
            }
            return counter;
        }
    }
    public InventorySlot FindItemOnInventory(Item _item)
    {
        for(int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item.Id == _item.Id)
            {
                return GetSlots[i]; 
            }
        }
        return null;
    }
    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item.Id <= -1)
            {
                GetSlots[i].UpdateSlot(_item, _amount);
                return GetSlots[i];
            }
        }
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        // Se for o mesmo slot, não faz nada
        if (item1 == item2)
            return;

        // Verifica se os itens são do mesmo tipo e stackable
        if (item1.ItemObject != null && item2.ItemObject != null && 
            item1.ItemObject == item2.ItemObject && 
            item1.ItemObject.stackable)
        {
            // Junta as quantidades no slot de destino
            item2.AddAmount(item1.amount);
            item1.RemoveItem();
        }
        else if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            // Troca normal de itens
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }
        // Se não puderem ser trocados, simplesmente não faz nada (cancela a operação)
    }

    // No script InventoryObject
    public void RemoveItem(ItemObject itemObject)
    {
        if (itemObject == null) return;

        Item item = new Item(itemObject);
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.Id == item.Id)
            {
                int amount = GetSlots[i].amount;
                GetSlots[i].UpdateSlot(null, 0);
                OnItemRemoved?.Invoke(itemObject, amount);
            }
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].amount);
            }
            stream.Close();
        }
    }
    
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Slots = new InventorySlot[30];
    public void Clear()
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            Slots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlot _slot);

[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized]
    public UserInterface parent;
    [System.NonSerialized]
    public GameObject slotDisplay;
    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;
    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;
    public Item item;
    public int amount;

    public ItemObject ItemObject
    {
        get
        {
            if(item.Id >= 0 && parent != null && parent.inventory != null && parent.inventory.database != null)
            {
                return parent.inventory.database.ItemObjects[item.Id];
            }
            return null;
        }
    }
    
    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);
    }
    
    public InventorySlot(Item _item, int _amount)
    {
        UpdateSlot(_item, _amount);
    }
    
    public void UpdateSlot(Item _item, int _amount)
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);
            
        item = _item;
        amount = _amount;
        
        if (OnAfterUpdate != null && this != null)
            OnAfterUpdate.Invoke(this);
    }

    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }
    
    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);
    }
    
    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if(AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0) 
            return true;
        for(int i = 0; i < AllowedItems.Length; i++)
        {
            if(_itemObject.type == AllowedItems[i])
                return true;
        }
        return false;
    }
}