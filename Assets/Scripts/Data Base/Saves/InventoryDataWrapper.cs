// InventoryData.cs
using System;
using UnityEngine;

// InventoryData.cs
[Serializable]
public class InventoryDataWrapper
{
    public Inventory inventory1;
    public Inventory inventory2;
    public int money1; // Dinheiro do primeiro inventário
    public int money2; // Dinheiro do segundo inventário

    public InventoryDataWrapper(InventoryObject inv1, InventoryObject inv2)
    {
        inventory1 = inv1.Container;
        inventory2 = inv2.Container;
        money1 = inv1.Money;
        money2 = inv2.Money;
    }
}
