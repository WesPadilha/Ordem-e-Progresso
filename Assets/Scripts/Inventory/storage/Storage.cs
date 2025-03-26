using System.Collections;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public GameObject storageUI;
    public ScreenController screenController;
    public Transform player;
    public Transform storage;
    public Camera storageCamera;
    public float interactionDistance = 3f;
    public float fixedDistance = 12f;
    public float movementSpeed = 5f;
    public GameObject hud;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private void OnMouseOver()
    {
        // Verifica se o jogador está no range e o estado do jogo
        if (Pause.GameIsPaused || Vector3.Distance(player.position, transform.position) > interactionDistance || screenController.IsAnyUIActive())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            OpenStorage();
        }
    }

    public void OpenStorage()
    {
        storageUI.SetActive(true); // Abre a UI de storage
        screenController.SetStorageState(true); // Marca o estado do storage

        // Armazena a posição e rotação original da câmera
        originalCameraPosition = storageCamera.transform.position;
        originalCameraRotation = storageCamera.transform.rotation;

        StartCoroutine(MoveCameraToStoragePosition()); // Move a câmera para a posição do Storage
        hud.SetActive(false);
    }

    public void CloseStorage()
    {
        storageUI.SetActive(false); // Fecha a UI de storage
        screenController.SetStorageState(false); // Reseta o estado do storage
        hud.SetActive(true);
    }

    private IEnumerator MoveCameraToStoragePosition()
    {
        float currentAngleY = storageCamera.transform.rotation.eulerAngles.y;
        Vector3 direction = new Vector3(Mathf.Sin(currentAngleY * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngleY * Mathf.Deg2Rad));
        Vector3 targetPosition = storage.position - direction * fixedDistance;
        targetPosition.y = storageCamera.transform.position.y; // Mantém a altura da câmera

        while (Vector3.Distance(storageCamera.transform.position, targetPosition) > 0.1f)
        {
            storageCamera.transform.position = Vector3.Lerp(storageCamera.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void Update()
    {
        // Fecha o storage se pressionar 'Esc'
        if (Input.GetKeyDown(KeyCode.Escape) && storageUI.activeSelf)
        {
            CloseStorage();
        }
    }
}