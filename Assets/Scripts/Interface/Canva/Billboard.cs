using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform Camera;
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.forward);
    }
}
