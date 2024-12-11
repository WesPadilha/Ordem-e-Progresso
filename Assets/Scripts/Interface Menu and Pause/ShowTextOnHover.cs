using UnityEngine;
using TMPro; // Adicione este namespace para usar o TextMeshPro

public class ShowTextOnHover : MonoBehaviour
{
    public TMP_Text hoverText; // Referência ao texto no Canvas (usando TextMeshPro)
    public string textToShow;  // Texto a ser exibido

    private void Start()
    {
        if (hoverText != null)
        {
            hoverText.enabled = false; // Certifique-se de que o texto está invisível inicialmente
        }
    }

    private void OnMouseEnter()
    {
        if (hoverText != null)
        {
            hoverText.text = textToShow; // Define o texto
            hoverText.enabled = true;    // Torna o texto visível
        }
    }

    private void OnMouseExit()
    {
        if (hoverText != null)
        {
            hoverText.enabled = false; // Esconde o texto
        }
    }
}
