// WeaponLoaderData.cs
using System;
using UnityEngine;

[Serializable]
public class WeaponLoaderData
{
    public int ammoCapacity;
    public int ammoCurrent;
    public string ammoType;

    public WeaponLoaderData(WeaponLoader loader)
    {
        ammoCapacity = loader.ammoCapacity;
        ammoCurrent = loader.ammoCurrent;

        if (loader is WeaponLoaderWithType withType)
        {
            ammoType = withType.ammoType.ToString();
        }
        else
        {
            ammoType = "None";
        }
    }
}
