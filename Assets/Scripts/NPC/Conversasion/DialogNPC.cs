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
    public NPCOrientationController npcOrientationController; // Referência ao controlador de orientação do NPC

    private bool isConversationActive = false; // Indica se a conversa está ativa
    private Vector3 conversationCameraPosition; // Posição da câmera durante a conversa
    private Quaternion conversationCameraRotation; // Rotação da câmera durante a conversa

    void Update()
    {
        // Verifica se a conversa foi finalizada e atualiza o status
        if (isConversationActive && !ConversationManager.Instance.IsConversationActive)
        {
            isConversationActive = false;
            EndConversation();
        }
    }

    private void OnMouseOver()
    {
        // Verifica se o jogo está pausado, se a UI está ativa ou se a distância é maior que a necessária
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

            // Faz o NPC rotacionar em direção ao jogador
            npcOrientationController.StartConversation();

            // Faz o jogador rotacionar em direção ao NPC
            RotatePlayerTowards(NPC);

            // Inscreve o evento de término da conversa
            if (ConversationManager.OnConversationEnded != null)
            {
                ConversationManager.OnConversationEnded += EndConversation;
            }
        }
    }

    private IEnumerator MoveCameraToFixedPosition()
    {
        float currentAngleY = camera.transform.rotation.eulerAngles.y;
        Vector3 direction = new Vector3(Mathf.Sin(currentAngleY * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngleY * Mathf.Deg2Rad));
        Vector3 targetPosition = NPC.position - direction * fixedDistance;
        targetPosition.y = camera.transform.position.y; // Mantém a altura atual da câmera

        while (Vector3.Distance(camera.transform.position, targetPosition) > 0.1f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Salva a posição e rotação da câmera ao final do movimento
        conversationCameraPosition = camera.transform.position;
        conversationCameraRotation = camera.transform.rotation;

        // Garante precisão ao final do movimento
        camera.transform.position = targetPosition;
    }

    public void EndConversation()
    {
        // Verifica se o objeto foi destruído
        if (this == null) return;

        // Finaliza a conversa e movimenta a câmera de volta
        screenController.EndConversation();
        StartCoroutine(ReturnCameraToConversationPosition());

        // Faz o NPC retornar à posição original
        npcOrientationController.EndConversation();

        // Desinscreve o evento para evitar problemas caso o objeto seja destruído
        ConversationManager.OnConversationEnded -= EndConversation;
    }

    private IEnumerator ReturnCameraToConversationPosition()
    {
        // Move a câmera de volta à posição salva durante a conversa
        while (Vector3.Distance(camera.transform.position, conversationCameraPosition) > 0.1f || Quaternion.Angle(camera.transform.rotation, conversationCameraRotation) > 0.1f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, conversationCameraPosition, movementSpeed * Time.deltaTime);
            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, conversationCameraRotation, movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante precisão ao final do movimento
        camera.transform.position = conversationCameraPosition;
        camera.transform.rotation = conversationCameraRotation;
    }

    private void RotatePlayerTowards(Transform target)
    {
        Vector3 direction = (target.position - player.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        player.rotation = lookRotation;
    }
}
