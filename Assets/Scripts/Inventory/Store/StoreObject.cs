using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Store", menuName = "InventorySystem/Store")]
public class StoreObject : ScriptableObject
{
    public ItemObject[] itemsForSale;
    public float sellPriceMultiplier = 0.7f; // Percentagem do valor original ao vender
}