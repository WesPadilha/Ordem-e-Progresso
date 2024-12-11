using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSelection : MonoBehaviour
{
    Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;  // Certifique-se de desabilitar o contorno inicialmente
    }

    private void OnMouseEnter()  // Ativa o contorno quando o mouse entra na área do objeto
    {
        outline.enabled = true;
    }

    private void OnMouseExit()  // Desativa o contorno quando o mouse sai da área do objeto
    {
        outline.enabled = false;
    }
}
