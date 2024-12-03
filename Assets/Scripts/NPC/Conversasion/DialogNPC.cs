using System.Collections;
using UnityEngine;
using DialogueEditor;

public class DialogNPC : MonoBehaviour
{
    public NPCConversation myConversation;
    public ScreenController screenController; // Referência ao ScreenController
    public Transform player; // Referência ao jogador
    public Transform NPC; // Referência ao NPC
    public Camera camera; // Referência à câmera principal
    public float interactionDistance = 3f; // Distância mínima para iniciar a conversa
    public float fixedDistance = 12f; // Distância fixa da câmera ao NPC
    public float movementSpeed = 5f; // Velocidade de movimento da câmera

    private bool isConversationActive = false; // Indica se a conversa está ativa
    private Vector3 originalCameraPosition; // Posição original da câmera
    private Quaternion originalCameraRotation; // Rotação original da câmera

    private void Start()
    {
        // Salva a posição e rotação originais da câmera
        originalCameraPosition = camera.transform.position;
        originalCameraRotation = camera.transform.rotation;
    }

    private void OnMouseOver()
    {
        // Verifica se o jogo está pausado ou se a UI está ativa
        if (Pause.GameIsPaused || Vector3.Distance(player.position, transform.position) > interactionDistance || screenController.IsAnyUIActive())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Inicia a conversa
            screenController.StartConversation();
            ConversationManager.Instance.StartConversation(myConversation);

            // Marca a conversa como ativa
            isConversationActive = true;

            // Move a câmera para a posição correta
            StartCoroutine(MoveCameraToFixedPosition());

            // Escuta o evento de término da conversa
            ConversationManager.OnConversationEnded += EndConversation;
        }
    }

    private IEnumerator MoveCameraToFixedPosition()
    {
        // A rotação da câmera é preservada (não mexe em Y ou qualquer outra rotação)
        float currentAngleY = camera.transform.rotation.eulerAngles.y;

        // Calcula a posição da câmera com base na rotação Y e a distância fixa
        Vector3 direction = new Vector3(Mathf.Sin(currentAngleY * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngleY * Mathf.Deg2Rad));
        Vector3 targetPosition = NPC.position - direction * fixedDistance;
        targetPosition.y = originalCameraPosition.y; // Mantém a altura original

        // Move a câmera suavemente para a posição calculada
        while (Vector3.Distance(camera.transform.position, targetPosition) > 0.1f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante a posição final precisa, mas sem mexer na rotação Y
        camera.transform.position = targetPosition;
    }

    public void EndConversation()
    {
        isConversationActive = false;

        // Finaliza a conversa no ScreenController
        screenController.EndConversation();

        // Retorna a câmera à posição e rotação originais
        StartCoroutine(ReturnCameraToOriginalPosition());

        // Remove o listener do evento
        ConversationManager.OnConversationEnded -= EndConversation;
    }

    private IEnumerator ReturnCameraToOriginalPosition()
    {
        // Move a câmera suavemente de volta à posição original, preservando a rotação
        while (Vector3.Distance(camera.transform.position, originalCameraPosition) > 0.1f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, originalCameraPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante a posição final precisa
        camera.transform.position = originalCameraPosition;

        // Mantém a rotação original da câmera
        camera.transform.rotation = originalCameraRotation;
    }
}
