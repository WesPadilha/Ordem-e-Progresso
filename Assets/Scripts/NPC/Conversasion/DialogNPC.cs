using System.Collections;
using UnityEngine;
using DialogueEditor;
using UnityEngine.AI;

public class DialogNPC : MonoBehaviour
{
    public NPCConversation myConversation;
    public ScreenController screenController;
    public Transform player;
    public Transform NPC;
    public Camera npcCamera;
    public float interactionDistance = 3f;
    public float fixedDistance = 12f;
    public float movementSpeed = 5f;
    public NPCOrientationController npcOrientationController;
    public GameObject hud;
    public CharacterData characterData; // Referência ao CharacterData

    private bool isConversationActive = false;
    private Vector3 conversationCameraPosition;
    private Quaternion conversationCameraRotation;

    void Update()
    {
        if (isConversationActive && !ConversationManager.Instance.IsConversationActive)
        {
            isConversationActive = false;
            EndConversation();
        }
    }

    private void OnMouseOver()
    {
        if (Pause.GameIsPaused || Vector3.Distance(player.position, transform.position) > interactionDistance || screenController.IsAnyUIActive() || screenController.IsStorageOpen())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            StartConversation();
        }
    }

    public void StartConversation()
    {
        if (isConversationActive || screenController.IsAnyUIActive())
            return;

        // Para o movimento do agente
        var agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath(); // Cancela qualquer caminho definido
            agent.isStopped = true; // Para a movimentação
        }

        screenController.StartConversation();
        ConversationManager.Instance.StartConversation(myConversation);
        
        // Define todas as habilidades no ConversationManager
        SetAllSkillsInConversationManager();

        isConversationActive = true;
        StartCoroutine(MoveCameraToFixedPosition());
        npcOrientationController.StartConversation();
        RotatePlayerTowards(NPC);

        if (ConversationManager.OnConversationEnded != null)
        {
            ConversationManager.OnConversationEnded += EndConversation;
        }

        hud.SetActive(!hud.activeSelf);
    }

    private void SetAllSkillsInConversationManager()
    {
        if (characterData == null) return;

        // Atributos
        ConversationManager.Instance.SetInt("Forca", characterData.strength);
        ConversationManager.Instance.SetInt("Intelecto", characterData.intellection);
        ConversationManager.Instance.SetInt("Sorte", characterData.luck);
        ConversationManager.Instance.SetInt("Inteligencia", characterData.intelligence);
        ConversationManager.Instance.SetInt("Carisma", characterData.charisma);
        ConversationManager.Instance.SetInt("Agilidade", characterData.agility);

        // Habilidades
        ConversationManager.Instance.SetInt("Arrombamento", characterData.arrombamento);
        ConversationManager.Instance.SetInt("Atletismo", characterData.atletismo);
        ConversationManager.Instance.SetInt("Ciencias", characterData.ciencias);
        ConversationManager.Instance.SetInt("Diplomacia", characterData.diplomacia);
        ConversationManager.Instance.SetInt("Eletrica", characterData.eletrica);
        ConversationManager.Instance.SetInt("Furtividade", characterData.furtividade);
        ConversationManager.Instance.SetInt("Geografia", characterData.geografia);
        ConversationManager.Instance.SetInt("Idiomas", characterData.idiomas);
        ConversationManager.Instance.SetInt("Intuicao", characterData.intuicao);
        ConversationManager.Instance.SetInt("Medicina", characterData.medicina);
        ConversationManager.Instance.SetInt("Mecanica", characterData.mecanica);
        ConversationManager.Instance.SetInt("Negociacao", characterData.negociacao);
        ConversationManager.Instance.SetInt("Religiao", characterData.religiao);
        ConversationManager.Instance.SetInt("Roubo", characterData.roubo);
    }

    private IEnumerator MoveCameraToFixedPosition()
    {
        float currentAngleY = npcCamera.transform.rotation.eulerAngles.y;
        Vector3 direction = new Vector3(Mathf.Sin(currentAngleY * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngleY * Mathf.Deg2Rad));
        Vector3 targetPosition = NPC.position - direction * fixedDistance;
        targetPosition.y = npcCamera.transform.position.y;

        while (Vector3.Distance(npcCamera.transform.position, targetPosition) > 0.1f)
        {
            npcCamera.transform.position = Vector3.Lerp(npcCamera.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        conversationCameraPosition = npcCamera.transform.position;
        conversationCameraRotation = npcCamera.transform.rotation;
        npcCamera.transform.position = targetPosition;
    }

    public void EndConversation()
    {
        if (this == null) return;

        screenController.EndConversation();

        var agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = false; // Permite movimento novamente
        }

        StartCoroutine(ReturnCameraToConversationPosition());
        npcOrientationController.EndConversation();
        ConversationManager.OnConversationEnded -= EndConversation;

        // Só ativa o HUD se a loja ou armazenamento não estiverem abertos
        if (!screenController.IsStorageOpen() && !screenController.IsStoreOpen())
        {
            hud.SetActive(true);
        }
    }

    private IEnumerator ReturnCameraToConversationPosition()
    {
        while (Vector3.Distance(npcCamera.transform.position, conversationCameraPosition) > 0.1f || Quaternion.Angle(npcCamera.transform.rotation, conversationCameraRotation) > 0.1f)
        {
            npcCamera.transform.position = Vector3.Lerp(npcCamera.transform.position, conversationCameraPosition, movementSpeed * Time.deltaTime);
            npcCamera.transform.rotation = Quaternion.Lerp(npcCamera.transform.rotation, conversationCameraRotation, movementSpeed * Time.deltaTime);
            yield return null;
        }

        npcCamera.transform.position = conversationCameraPosition;
        npcCamera.transform.rotation = conversationCameraRotation;
    }

    private void RotatePlayerTowards(Transform target)
    {
        Vector3 direction = (target.position - player.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        player.rotation = lookRotation;
    }
}