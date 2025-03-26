using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class VendorUI : MonoBehaviour
{
    public InventoryObject playerInventory; // Referência ao inventário do jogador
    public ItemObject[] itemsForSale; // Itens que o vendedor está vendendo
    public GameObject[] vendorSlots; // Slots do vendedor
    public Sprite defaultIcon; // Imagem padrão para slots vazios
    public PlayerStatus playerStatus;

    public InventoryObject storeInventory; // Dinheiro inicial do vendedor
    public TextMeshProUGUI vendorMoneyText; // Referência ao texto que exibe o dinheiro do vendedor

    void Start()
    {
        playerStatus.UpdateMoneyDisplay(); // Atualiza o display de dinheiro do jogador
        UpdateVendorMoneyDisplay(); // Atualiza o display de dinheiro do vendedor
        DistributeItems(); // Distribui os itens nos slots
    }

    // Atualiza o display de dinheiro do vendedor
    public void UpdateVendorMoneyDisplay()
    {
        if (vendorMoneyText != null)
        {
            vendorMoneyText.text = "$" + storeInventory.Money.ToString();
        }
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

    public void OnDragEnd(GameObject obj)
    {
        if (MouseData.interfaceMouseIsOver == null)
        {
            // Se o item foi arrastado para fora da interface, cancela a ação
            return;
        }

        if (MouseData.slotHoveredOver != null)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            VendorSlot vendorSlot = obj.GetComponent<VendorSlot>();

            if (vendorSlot != null && mouseHoverSlotData != null)
            {
                if (MouseData.interfaceMouseIsOver is VendorUI)
                {
                    // Se o item foi arrastado para a loja, tenta vender o item
                    SellItemToVendor(vendorSlot.item);
                }
                else
                {
                    // Se o item foi arrastado para o inventário, tenta comprar o item
                    BuyItemFromVendor(vendorSlot.item);
                }
            }
        }
    }

    // Método para comprar um item da loja
    public void BuyItemFromVendor(ItemObject item)
    {
        if (item != null && playerInventory.Money >= item.price)
        {
            // Transfere o dinheiro do jogador para a loja
            playerInventory.Money -= item.price;
            storeInventory.Money += item.price;

            // Adiciona o item ao inventário do jogador
            playerInventory.AddItem(new Item(item), 1);

            // Remove o item da loja
            RemoveItemFromVendor(item);

            // Atualiza os displays de dinheiro
            playerStatus.UpdateMoneyDisplay();
            UpdateVendorMoneyDisplay();
        }
        else
        {
            Debug.Log("O jogador não tem dinheiro suficiente para comprar este item.");
        }
    }

    // Método para vender um item para a loja
    public void SellItemToVendor(ItemObject item)
    {
        if (item != null && storeInventory.Money >= item.price)
        {
            // Transfere o dinheiro da loja para o jogador
            storeInventory.Money -= item.price;
            playerInventory.Money += item.price;

            // Remove o item do inventário do jogador
            playerInventory.RemoveItem(item);

            // Adiciona o item à loja
            AddItemToVendor(item);

            // Atualiza os displays de dinheiro
            playerStatus.UpdateMoneyDisplay();
            UpdateVendorMoneyDisplay();
        }
        else
        {
            Debug.Log("A loja não tem dinheiro suficiente para comprar este item.");
        }
    }

    // Método para remover o item da loja
    private void RemoveItemFromVendor(ItemObject item)
    {
        for (int i = 0; i < itemsForSale.Length; i++)
        {
            if (itemsForSale[i] == item)
            {
                itemsForSale[i] = null;
                DistributeItems(); // Atualiza a exibição dos itens na loja
                break;
            }
        }
    }

    // Método para adicionar o item à loja
    private void AddItemToVendor(ItemObject item)
    {
        for (int i = 0; i < itemsForSale.Length; i++)
        {
            if (itemsForSale[i] == null)
            {
                itemsForSale[i] = item;
                DistributeItems(); // Atualiza a exibição dos itens na loja
                break;
            }
        }
    }
}