// PlayerLocationData.cs
using UnityEngine;
using System;

[System.Serializable]
public class PlayerLocationData
{
    public float x, y, z;

    public PlayerLocationData(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(x, y, z);
    }
}
