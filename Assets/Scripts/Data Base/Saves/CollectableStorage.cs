using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectableStorage : MonoBehaviour
{
    public string uniqueID;
    public bool alreadyCollected = false;

    private void Reset()
    {
        uniqueID = Guid.NewGuid().ToString(); // Gera ID único no Inspector
    }

    public void MarkCollected()
    {
        alreadyCollected = true;
    }
}
