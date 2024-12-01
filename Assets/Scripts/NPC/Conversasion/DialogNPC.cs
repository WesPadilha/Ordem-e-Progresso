using System.Collections; // Adicione esta linha
using UnityEngine;
using DialogueEditor;

public class DialogNPC : MonoBehaviour
{
    public NPCConversation myConversation;
    public ScreenController screenController; // Referência ao ScreenController
    public Transform player; // Referência ao jogador
    public Transform NPC;
    public Camera camera;
    public float interactionDistance = 3f; // Distância mínima para iniciar a conversa
    public float distanceBehindNPC = 5f; // Distância desejada atrás do NPC
    public float rotationSpeed = 5f; // Velocidade de rotação da câmera

    private bool isConversationActive = false; // Definindo a variável isConversationActive
    private Vector3 originalCameraPosition; // Para armazenar a posição original da câmera
    private Quaternion originalCameraRotation; // Para armazenar a rotação original da câmera

    private void Start()
    {
        // Salva a posição e rotação originais da câmera
        originalCameraPosition = camera.transform.position;
        originalCameraRotation = camera.transform.rotation;
    }

    private void OnMouseOver()
    {
        // Verifica se o jogo está pausado ou se uma UI está ativa
        if (Pause.GameIsPaused || Vector3.Distance(player.position, transform.position) > interactionDistance || screenController.IsAnyUIActive())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Inicia a conversa
            screenController.StartConversation();
            ConversationManager.Instance.StartConversation(myConversation);

            // Marca a conversa como ativa
            isConversationActive = true;

            // Salva a posição e rotação atuais da câmera para retornar após o diálogo
            originalCameraPosition = camera.transform.position;
            originalCameraRotation = camera.transform.rotation;

            // Move a câmera para 5 metros atrás do NPC, mantendo a altura original
            Vector3 directionBehind = (camera.transform.position - NPC.position).normalized;
            camera.transform.position = NPC.position + directionBehind * distanceBehindNPC;

            // Mantém a altura original da câmera
            camera.transform.position = new Vector3(camera.transform.position.x, originalCameraPosition.y, camera.transform.position.z);

            // Rotaciona a câmera para focar no NPC, mas sem alinhar totalmente
            StartCoroutine(RotateCameraToNPC());

            // Escuta quando a conversa termina
            ConversationManager.OnConversationEnded += EndConversation;
        }
    }

    private IEnumerator RotateCameraToNPC()
    {
        // Calcula a rotação do NPC, mas mantendo um ângulo de visão "natural" para a câmera
        Vector3 directionToNPC = NPC.position - camera.transform.position;
        float targetAngleY = Mathf.Atan2(directionToNPC.x, directionToNPC.z) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(originalCameraRotation.eulerAngles.x, targetAngleY, originalCameraRotation.eulerAngles.z);

        // Rotaciona suavemente no eixo Y para focar no NPC
        while (Quaternion.Angle(camera.transform.rotation, targetRotation) > 0.1f)
        {
            camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante que a rotação final seja precisa
        camera.transform.rotation = targetRotation;
    }

    public void EndConversation()
    {
        isConversationActive = false;

        screenController.EndConversation();

        // Retorna a câmera à sua posição e rotação originais
        camera.transform.position = originalCameraPosition;
        camera.transform.rotation = originalCameraRotation;

        // Remove o listener para evitar múltiplas assinaturas
        ConversationManager.OnConversationEnded -= EndConversation;
    }
}
