using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardImage : MonoBehaviour
{
    public Transform cameraTransform; // Referência à câmera
    public RectTransform rectTransform; // Referência ao RectTransform do Canvas
    public Transform objectToFollow; // O GameObject que a imagem deve ficar voltada

    void LateUpdate()
    {
        // Certifica-se de que a imagem dentro do Canvas sempre fica voltada para o objeto associado
        if (objectToFollow != null)
        {
            // Mantém a posição da imagem dentro do Canvas, mas rotaciona para olhar para o objeto.
            Vector3 direction = objectToFollow.position - cameraTransform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Aplique a rotação ao objeto dentro do Canvas
            rectTransform.rotation = rotation;
        }
    }
}
