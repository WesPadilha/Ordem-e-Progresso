using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class VendorUI : MonoBehaviour
{
    [Header("Store Settings")]
    public List<ItemObject> itemsForSale;
    public int initialVendorMoney = 1000;
    
    [Header("References")]
    public StoreInterface storeInterface;
    public DynamicInterface playerInterface;
    public PlayerStatus playerStatus;
    public TextMeshProUGUI vendorMoneyText;

    private bool isInitialized = false;

    private void Start()
    {
        if (!isInitialized)
        {
            InitializeStore();
        }
    }

    private void InitializeStore()
    {
        if (isInitialized) return;

        // Initialize money first
        storeInterface.inventory.Money = initialVendorMoney;
        UpdateVendorMoneyDisplay();
        
        // Clear inventory safely
        if (storeInterface.inventory != null)
        {
            storeInterface.inventory.Clear();
        }
        
        // Set references
        storeInterface.playerInventory = playerInterface.inventory;
        storeInterface.playerStatus = playerStatus;
        storeInterface.vendorUI = this;
        
        // Add items
        Dictionary<ItemObject, int> itemCounts = new Dictionary<ItemObject, int>();
        
        foreach (ItemObject item in itemsForSale)
        {
            if (item != null)
            {
                if (itemCounts.ContainsKey(item))
                {
                    itemCounts[item]++;
                }
                else
                {
                    itemCounts.Add(item, 1);
                }
            }
        }
        
        foreach (var kvp in itemCounts)
        {
            ItemObject item = kvp.Key;
            int count = kvp.Value;
            
            if (item.stackable)
            {
                storeInterface.inventory.AddItem(new Item(item), count);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    storeInterface.inventory.AddItem(new Item(item), 1);
                }
            }
        }

        isInitialized = true;
        
        // Force UI update after initialization
        if (storeInterface != null)
        {
            storeInterface.UpdateAllSlotDisplays();
        }
    }

    public void UpdateVendorMoneyDisplay()
    {
        vendorMoneyText.text = "$" + storeInterface.inventory.Money;
    }
}