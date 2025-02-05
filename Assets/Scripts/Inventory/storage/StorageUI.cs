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
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Length && items[i] != null) // Verifica se há um item no array
            {
                // Define o item e o ícone no slot
                slots[i].GetComponent<StorageSlot>().SetItem(items[i]);
            }
            else
            {
                // Se não houver itens suficientes, define o slot como vazio com a imagem padrão
                slots[i].GetComponent<StorageSlot>().ClearSlot(defaultIcon);
            }
        }
    }

    // Método para transferir o item do slot para o inventário do jogador
    public void TransferItemToInventory(ItemObject item)
    {
        if (item != null)
        {
            playerInventory.AddItem(new Item(item), 1);
        }
    }
}