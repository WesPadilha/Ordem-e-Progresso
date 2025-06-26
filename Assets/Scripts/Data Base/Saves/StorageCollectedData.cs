// StorageCollectedData.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StorageCollectedData
{
    public List<string> collectedStorageIDs = new List<string>();

    public StorageCollectedData(List<string> ids)
    {
        collectedStorageIDs = ids;
    }
}
