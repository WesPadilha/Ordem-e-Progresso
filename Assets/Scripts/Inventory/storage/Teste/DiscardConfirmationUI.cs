using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiscardConfirmationUI : MonoBehaviour
{
    public GameObject confirmationPanel;
    public Button yesButton;
    public Button noButton;
    public Slider quantitySlider;
    public TextMeshProUGUI quantityText;

    private InventorySlot slotToDiscard;

    private void Start()
    {
        confirmationPanel.SetActive(false);
        quantitySlider.gameObject.SetActive(false);

        yesButton.onClick.AddListener(ConfirmDiscard);
        noButton.onClick.AddListener(CancelDiscard);
    }

    private void Update()
    {
        if (quantityText == null) return; // Add this safety check

        if (quantitySlider != null && quantitySlider.gameObject.activeSelf && 
            slotToDiscard != null && slotToDiscard.ItemObject != null && 
            slotToDiscard.ItemObject.stackable)
        {
            quantityText.text = "Quantidade: " + Mathf.RoundToInt(quantitySlider.value).ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }

    public void AskForConfirmation(InventorySlot slot)
    {
        if (slot == null || slot.ItemObject == null) return;

        if (slot.ItemObject.type == ItemType.Quest)
        {
            // Não permite descartar itens de missão
            Debug.Log("Não é possível descartar itens de missão!");
            return;
        }
        
        slotToDiscard = slot;
        ItemObject itemObj = slot.ItemObject;

        if (quantitySlider != null)
        {
            if (itemObj.stackable && slot.amount > 1)
            {
                quantitySlider.gameObject.SetActive(true);
                quantitySlider.minValue = 1;
                quantitySlider.maxValue = slot.amount;
                quantitySlider.value = 1;
            }
            else
            {
                quantitySlider.gameObject.SetActive(false);
            }
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    private void ConfirmDiscard()
    {
        int amountToDiscard = 1;

        if (quantitySlider.gameObject.activeSelf)
        {
            amountToDiscard = Mathf.RoundToInt(quantitySlider.value);
        }

        ItemObject itemObject = slotToDiscard.ItemObject;
        InventoryObject inventory = slotToDiscard.parent.inventory;

        if (slotToDiscard.amount <= amountToDiscard)
        {
            slotToDiscard.RemoveItem();
            inventory.NotifyItemRemoved(itemObject, amountToDiscard);
        }
        else
        {
            slotToDiscard.UpdateSlot(slotToDiscard.item, slotToDiscard.amount - amountToDiscard);
            inventory.NotifyItemRemoved(itemObject, amountToDiscard);
        }

        confirmationPanel.SetActive(false);
    }


    private void CancelDiscard()
    {
        slotToDiscard = null;
        confirmationPanel.SetActive(false);
    }
}
