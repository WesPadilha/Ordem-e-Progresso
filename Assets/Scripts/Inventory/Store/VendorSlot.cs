using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VendorSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon; // Referência ao ícone do item
    public TextMeshProUGUI priceText; // Referência ao texto do preço
    [System.NonSerialized] public ItemObject item; // Item associado ao slot (não serializado)

    private VendorUI vendorUI;

    void Start()
    {
        vendorUI = GetComponentInParent<VendorUI>();
    }

    // Define o item no slot
    public void SetItem(ItemObject newItem)
    {
        item = newItem;
        if (item != null && item.uiDisplay != null)
        {
            icon.sprite = item.uiDisplay; // Define o ícone do item
            icon.enabled = true; // Ativa a exibição do ícone
            priceText.text = "$" + item.price; // Exibe o preço do item
        }
    }

    // Limpa o slot e define a imagem padrão
    public void ClearSlot(Sprite defaultIcon)
    {
        item = null;
        icon.sprite = defaultIcon; // Define a imagem padrão
        icon.enabled = true; // Mantém a imagem visível
        priceText.text = ""; // Remove o texto do preço
    }

    // Muda a cor do ícone
    public void SetIconColor(Color color)
    {
        icon.color = color;
    }

    // Quando o slot é clicado, tenta comprar o item
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null)
        {
            vendorUI.TryBuyItem(item); // Tenta comprar o item
        }
    }
}