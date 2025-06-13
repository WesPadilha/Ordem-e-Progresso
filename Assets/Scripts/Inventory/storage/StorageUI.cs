using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    public InventoryObject playerInventory; // Referência ao inventário do jogador
    public ItemObject[] items; // Lista de itens que podem ser colocados no armazenamento
    public GameObject[] slots; // Array de slots no armazenamento
    public Sprite defaultIcon; // Imagem padrão para slots vazios

    void Start()
    {
        // Distribui os itens aleatoriamente nos slots
        DistributeItems();
    }

    // Distribui os itens aleatoriamente nos slots
    private void DistributeItems()
    {
        foreach (ItemObject item in items)
        {
            if (item == null) continue;

            bool added = false;

            if (item.stackable)
            {
                foreach (var slotObj in slots)
                {
                    var slot = slotObj.GetComponent<StorageSlot>();
                    if (slot.GetItem() == item)
                    {
                        slot.SetItem(item, 1);
                        added = true;
                        break;
                    }
                }
            }

            if (!added)
            {
                foreach (var slotObj in slots)
                {
                    var slot = slotObj.GetComponent<StorageSlot>();
                    if (slot.GetItem() == null)
                    {
                        slot.SetItem(item, 1);
                        break;
                    }
                }
            }
        }

        // Preenche os slots restantes com imagem padrão
        foreach (var slotObj in slots)
        {
            var slot = slotObj.GetComponent<StorageSlot>();
            if (slot.GetItem() == null)
                slot.ClearSlot(defaultIcon);
        }
    }
    
    public void TransferItemToInventory(ItemObject item)
    {
        if (item != null)
        {
            playerInventory.AddItem(new Item(item), 1);
        }
    }
}