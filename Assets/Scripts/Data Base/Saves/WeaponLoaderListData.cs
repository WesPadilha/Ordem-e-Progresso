// WeaponLoaderListData.cs
using System;
using System.Collections.Generic;

[Serializable]
public class WeaponLoaderListData
{
    public List<WeaponLoaderData> weaponLoaders = new List<WeaponLoaderData>();

    public WeaponLoaderListData(List<WeaponLoader> scriptableObjects)
    {
        foreach (var loader in scriptableObjects)
        {
            weaponLoaders.Add(new WeaponLoaderData(loader));
        }
    }
}
