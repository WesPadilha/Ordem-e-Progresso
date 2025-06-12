using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]

public class WeaponLoader : ScriptableObject
{
    [Header("Ammo Settings")]
    public int ammoCapacity = 0;
    public int ammoCurrent = 0;
}
