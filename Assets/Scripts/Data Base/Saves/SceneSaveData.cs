using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class SceneSaveData
{
    public string sceneName;

    public SceneSaveData(string name)
    {
        sceneName = name;
    }
}
