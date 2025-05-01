using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuantitySelectorUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject selectorPanel;
    public TextMeshProUGUI itemNameText;
    public Slider quantitySlider;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI totalPriceText;
    public Button confirmButton;
    public Button cancelButton;

    private InventorySlot sourceSlot;
    private InventorySlot targetSlot;
    private bool isBuying;
    private StoreInterface storeInterface;

    private void Awake()
    {
        selectorPanel.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        quantitySlider.onValueChanged.AddListener(UpdateQuantityDisplay);
    }

    public void ShowSelector(InventorySlot source, InventorySlot target, bool buying)
    {
        sourceSlot = source;
        targetSlot = target;
        isBuying = buying;

        itemNameText.text = source.ItemObject.name;
        quantitySlider.minValue = 1;
        quantitySlider.maxValue = source.amount;
        quantitySlider.value = 1;

        UpdateQuantityDisplay(1);
        selectorPanel.SetActive(true);
        
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = buying ? "Comprar" : "Vender";
    }

    private void UpdateQuantityDisplay(float value)
    {
        int quantity = Mathf.RoundToInt(value);
        quantityText.text = quantity.ToString();
        
        int totalPrice = quantity * sourceSlot.ItemObject.price;
        string transactionType = isBuying ? "Custo total" : "Ganho total";
        totalPriceText.text = $"{transactionType}: ${totalPrice}";
    }

    private void OnConfirm()
    {
        int quantity = Mathf.RoundToInt(quantitySlider.value);
        storeInterface.TransferItem(sourceSlot, targetSlot, quantity, isBuying);
        selectorPanel.SetActive(false);
    }

    private void OnCancel()
    {
        selectorPanel.SetActive(false);
    }

    public void SetStoreInterface(StoreInterface storeInterface)
    {
        this.storeInterface = storeInterface;
    }
}