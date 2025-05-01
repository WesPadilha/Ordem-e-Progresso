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
        if (quantitySlider.gameObject.activeSelf && slotToDiscard != null && slotToDiscard.ItemObject.stackable)
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
        slotToDiscard = slot;
        
        ItemObject itemObj = slot.ItemObject;

        if (itemObj != null && itemObj.stackable && slot.amount > 1)
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

        confirmationPanel.SetActive(true);
    }

    private void ConfirmDiscard()
    {
        int amountToDiscard = 1;

        if (quantitySlider.gameObject.activeSelf)
        {
            amountToDiscard = Mathf.RoundToInt(quantitySlider.value);
        }

        if (slotToDiscard.amount <= amountToDiscard)
        {
            slotToDiscard.RemoveItem();
        }
        else
        {
            slotToDiscard.UpdateSlot(slotToDiscard.item, slotToDiscard.amount - amountToDiscard);
        }

        confirmationPanel.SetActive(false);
    }

    private void CancelDiscard()
    {
        slotToDiscard = null;
        confirmationPanel.SetActive(false);
    }
}
