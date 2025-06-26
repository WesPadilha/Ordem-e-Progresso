using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StorageXPStateData
{
    public string uniqueID;
    public bool hasGivenXP;

    public StorageXPStateData(string id, bool xpGiven)
    {
        uniqueID = id;
        hasGivenXP = xpGiven;
    }
}

[Serializable]
public class StorageXPStateListData
{
    public List<StorageXPStateData> storageXPStates = new List<StorageXPStateData>();

    public StorageXPStateListData() { }

    public StorageXPStateListData(List<Storage> storages)
    {
        foreach(var storage in storages)
        {
            storageXPStates.Add(new StorageXPStateData(storage.uniqueID, storage.HasGivenXP()));
        }
    }
}
