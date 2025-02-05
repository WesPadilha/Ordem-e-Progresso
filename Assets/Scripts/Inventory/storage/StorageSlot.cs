using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StorageSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon; // Referência ao componente Image do slot
    private ItemObject item; // Item associado ao slot

    private StorageUI storageUI;

    void Start()
    {
        storageUI = GetComponentInParent<StorageUI>();
    }

    // Define o item no slot
    public void SetItem(ItemObject newItem)
    {
        item = newItem;
        if (item != null && item.uiDisplay != null) // Verifica se o item e o ícone são válidos
        {
            icon.sprite = item.uiDisplay; // Define o ícone do item
            icon.enabled = true; // Ativa a exibição do ícone
        }
    }

    // Limpa o slot e define a imagem padrão
    public void ClearSlot(Sprite defaultIcon)
    {
        item = null;
        icon.sprite = defaultIcon; // Define a imagem padrão
        icon.enabled = true; // Mantém a imagem visível
    }

    // Quando o slot é clicado, transfere o item para o inventário do jogador
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null)
        {
            storageUI.TransferItemToInventory(item); // Transfere o item para o inventário
            ClearSlot(storageUI.defaultIcon); // Limpa o slot e define a imagem padrão
        }
    }
}