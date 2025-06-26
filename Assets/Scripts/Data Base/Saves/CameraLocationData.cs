using UnityEngine;
using System;

[Serializable]
public class CameraLocationData
{
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;

    public CameraLocationData(Vector3 position, Quaternion rotation)
    {
        posX = position.x;
        posY = position.y;
        posZ = position.z;

        rotX = rotation.x;
        rotY = rotation.y;
        rotZ = rotation.z;
        rotW = rotation.w;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(rotX, rotY, rotZ, rotW);
    }
}