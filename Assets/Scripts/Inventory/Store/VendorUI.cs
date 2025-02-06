using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendorUI : MonoBehaviour
{
    public InventoryObject playerInventory; // Referência ao inventário do jogador
    public ItemObject[] itemsForSale; // Itens que o vendedor está vendendo
    public GameObject[] vendorSlots; // Slots do vendedor
    public Sprite defaultIcon; // Imagem padrão para slots vazios
    public PlayerStatus playerStatus;

    void Start()
    {
        playerStatus.UpdateMoneyDisplay(); // Atualiza o display de dinheiro
        DistributeItems(); // Distribui os itens nos slots
    }

    // Distribui os itens nos slots do vendedor
    private void DistributeItems()
    {
        for (int i = 0; i < vendorSlots.Length; i++)
        {
            if (i < itemsForSale.Length && itemsForSale[i] != null)
            {
                // Passa o ItemObject diretamente para o slot
                vendorSlots[i].GetComponent<VendorSlot>().SetItem(itemsForSale[i]);
            }
            else
            {
                // Se não houver itens suficientes, define o slot como vazio com a imagem padrão
                vendorSlots[i].GetComponent<VendorSlot>().ClearSlot(defaultIcon);
            }
        }
    }

    // Tenta comprar um item do vendedor
    public void TryBuyItem(ItemObject item)
    {
        if (playerInventory.Money >= item.price)
        {
            playerInventory.Money -= item.price; // Deduz o dinheiro
            playerInventory.AddItem(new Item(item), 1); // Adiciona o item ao inventário
            playerStatus.UpdateMoneyDisplay(); // Atualiza o display de dinheiro
        }
        else
        {
            StartCoroutine(ShowInsufficientFunds(item)); // Mostra feedback de dinheiro insuficiente
        }
    }

    // Mostra feedback de dinheiro insuficiente
    private IEnumerator ShowInsufficientFunds(ItemObject item)
    {
        VendorSlot slot = FindSlotByItem(item);
        if (slot != null)
        {
            slot.SetIconColor(Color.red); // Muda a cor do ícone para vermelho
            yield return new WaitForSeconds(2); // Espera 2 segundos
            slot.SetIconColor(Color.white); // Volta a cor do ícone para branco
        }
    }

    // Encontra o slot que contém o item
    private VendorSlot FindSlotByItem(ItemObject item)
    {
        foreach (GameObject slot in vendorSlots)
        {
            VendorSlot vendorSlot = slot.GetComponent<VendorSlot>();
            if (vendorSlot.item == item)
            {
                return vendorSlot;
            }
        }
        return null;
    }

    // Vende um item do inventário do jogador
    public void SellItem(ItemObject item)
    {
        if (item != null)
        {
            playerInventory.Money += item.price; // Adiciona o dinheiro
            playerInventory.RemoveItem(item); // Passa o ItemObject diretamente
            playerStatus.UpdateMoneyDisplay(); // Atualiza o display de dinheiro
        }
    }
}