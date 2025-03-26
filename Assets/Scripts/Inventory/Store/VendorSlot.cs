using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VendorSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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
            vendorUI.BuyItemFromVendor(item); // Tenta comprar o item
        }
    }

    // Inicia o arrasto do item
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            MouseData.tempItemBeingDragged = CreateTempItem();
        }
    }

    // Atualiza a posição do item arrastado
    public void OnDrag(PointerEventData eventData)
    {
        if (MouseData.tempItemBeingDragged != null)
        {
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    // Finaliza o arrasto do item
    public void OnEndDrag(PointerEventData eventData)
    {
        if (MouseData.tempItemBeingDragged != null)
        {
            Destroy(MouseData.tempItemBeingDragged);
            vendorUI.OnDragEnd(gameObject);
        }
    }

    // Cria um item temporário para ser arrastado
    private GameObject CreateTempItem()
    {
        GameObject tempItem = new GameObject();
        var rt = tempItem.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
        tempItem.transform.SetParent(transform.parent);
        var img = tempItem.AddComponent<Image>();
        img.sprite = icon.sprite;
        img.raycastTarget = false;
        return tempItem;
    }
}