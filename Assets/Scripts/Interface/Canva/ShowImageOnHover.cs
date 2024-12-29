using UnityEngine;
using UnityEngine.UI; // Para usar componentes de UI

public class ShowImageOnHover : MonoBehaviour
{
    public RectTransform hoverImage; // Referência à imagem no Canvas
    public float moveDistanceY = 500f; // Distância que a imagem deve subir no eixo Y
    private Vector2 originalPosition; // Posição original da imagem

    private void Start()
    {
        if (hoverImage != null)
        {
            originalPosition = hoverImage.anchoredPosition; // Armazena a posição inicial
        }
    }

    private void OnMouseEnter()
    {
        if (hoverImage != null)
        {
            hoverImage.anchoredPosition = originalPosition + new Vector2(0, moveDistanceY); // Move a imagem para cima
        }
    }

    private void OnMouseExit()
    {
        if (hoverImage != null)
        {
            hoverImage.anchoredPosition = originalPosition; // Restaura a posição original
        }
    }
}
