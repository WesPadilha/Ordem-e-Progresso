using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public float cameraMoveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float edgeBoundary = 10f; // Distância do canto para iniciar o movimento
    public Vector3 cameraOffset = new Vector3(0, 10, -10); // Posição isométrica inicial
    public float zoomSpeed = 2f; // Velocidade do zoom
    public float minY = 7f; // Altura mínima de zoom
    private float maxY; // Armazena a altura inicial para limitar o zoom para trás

    private void Start()
    {
        // Configura a posição inicial da câmera em relação ao alvo
        transform.position = cameraOffset;
        transform.rotation = Quaternion.Euler(60, 0, 0); // Ângulo isométrico
        maxY = transform.position.y; // Define a posição Y original como limite máximo
    }

    private void Update()
    {
        // Movimento da câmera ao atingir as bordas da tela
        Vector3 direction = Vector3.zero;

        if (Input.mousePosition.x > Screen.width - edgeBoundary)
            direction += transform.right;
        if (Input.mousePosition.x < edgeBoundary)
            direction -= transform.right;

        if (Input.mousePosition.y > Screen.height - edgeBoundary)
            direction += new Vector3(transform.forward.x, 0, transform.forward.z);
        if (Input.mousePosition.y < edgeBoundary)
            direction -= new Vector3(transform.forward.x, 0, transform.forward.z);

        transform.position += direction.normalized * cameraMoveSpeed * Time.deltaTime;

        // Rotação ao segurar o botão do meio do mouse (scroll)
        if (Input.GetMouseButton(2))
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.RotateAround(transform.position, Vector3.up, horizontalRotation);
        }

        // Zoom ao scrollar para frente e para trás
        float scrollInput = Input.mouseScrollDelta.y;
        if (scrollInput != 0f)
        {
            float newY = Mathf.Clamp(transform.position.y - scrollInput * zoomSpeed, minY, maxY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
