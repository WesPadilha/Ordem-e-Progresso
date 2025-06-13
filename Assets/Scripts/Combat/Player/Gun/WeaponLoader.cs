using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item munição")]

public class WeaponLoader : ScriptableObject
{
    [Header("Ammo Settings")]
    public int ammoCapacity = 0;
    public int ammoCurrent = 0;
}

[CreateAssetMenu(fileName = "New Weapon Loader", menuName = "Inventory System/Items/Weapon Loader With Type")]
public class WeaponLoaderWithType : WeaponLoader
{
    public AmmoType ammoType = AmmoType.None; // <- Define tipo da munição carregada
}

