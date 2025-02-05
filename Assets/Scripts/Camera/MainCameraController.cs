using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public float cameraMoveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float edgeBoundary = 10f; // Distância do canto para iniciar o movimento
    public Vector3 cameraOffset = new Vector3(0, 10, -10); // Posição isométrica inicial
    public float zoomSpeed = 2f; // Velocidade do zoom
    public float minY = 7f; // Altura mínima de zoom
    private float maxY; // Armazena a altura inicial para limitar o zoom para trás
    private float currentZoomOffsetY; // Offset atual de zoom

    public ScreenController screenController; // Referência ao ScreenController

    private void Start()
    {
        maxY = cameraOffset.y; // Define o limite superior com base na posição inicial
        currentZoomOffsetY = cameraOffset.y; // Inicializa o offset de zoom
        transform.position = player.position + cameraOffset;
        transform.rotation = Quaternion.Euler(60, 0, 0); // Ângulo isométrico
    }

    private void Update()
    {
        // Bloqueia movimentação da câmera se alguma UI estiver ativa
        if (screenController != null && screenController.IsAnyUIActive() || screenController.IsStorageOpen())
            return;

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

        // Zoom com ajuste de altura
        float scrollInput = Input.mouseScrollDelta.y;
        if (scrollInput != 0f)
        {
            currentZoomOffsetY = Mathf.Clamp(currentZoomOffsetY - scrollInput * zoomSpeed, minY, maxY);
        }

        // Ajusta a posição Y da câmera para acompanhar o jogador no eixo Y, aplicando o zoom
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Max(player.position.y + currentZoomOffsetY, minY);
        transform.position = newPosition;
    }
}
