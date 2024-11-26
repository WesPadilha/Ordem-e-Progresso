using UnityEngine;
using DialogueEditor;

public class DialogNPC : MonoBehaviour
{
    public NPCConversation myConversation;
    public ScreenController screenController; // Referência ao ScreenController
    public Transform player; // Referência ao jogador
    public float interactionDistance = 3f; // Distância mínima para iniciar a conversa

    private bool isConversationActive = false; // Definindo a variável isConversationActive

    private void OnMouseOver()
    {
        // Verifica se o jogador está próximo o suficiente
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Inicia a conversa e notifica o ScreenController
                screenController.StartConversation();
                ConversationManager.Instance.StartConversation(myConversation);

                // Marca a conversa como ativa
                isConversationActive = true;

                // Escuta quando a conversa termina
                ConversationManager.OnConversationEnded += EndConversation;
            }
        }
    }

    public void EndConversation()
    {
        isConversationActive = false; 
	
	    screenController.EndConversation();

        // Remove o listener para evitar múltiplas assinaturas
        ConversationManager.OnConversationEnded -= EndConversation;
    }

    private void Update()
    {
        // Sai da conversa ao apertar ESC
        if (Input.GetKeyDown(KeyCode.Escape) && ConversationManager.Instance != null && isConversationActive)
        {
            ConversationManager.Instance.EndConversation();
            EndConversation(); // Certifica-se de notificar o ScreenController
        }
    }
}
