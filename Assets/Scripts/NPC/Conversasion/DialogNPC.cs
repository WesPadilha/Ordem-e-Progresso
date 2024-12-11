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
    public Camera camera;
    public float interactionDistance = 3f;
    public float fixedDistance = 12f;
    public float movementSpeed = 5f;
    public NPCOrientationController npcOrientationController;

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
        if (Pause.GameIsPaused || Vector3.Distance(player.position, transform.position) > interactionDistance || screenController.IsAnyUIActive())
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

        isConversationActive = true;
        StartCoroutine(MoveCameraToFixedPosition());
        npcOrientationController.StartConversation();
        RotatePlayerTowards(NPC);

        if (ConversationManager.OnConversationEnded != null)
        {
            ConversationManager.OnConversationEnded += EndConversation;
        }
    }

    private IEnumerator MoveCameraToFixedPosition()
    {
        float currentAngleY = camera.transform.rotation.eulerAngles.y;
        Vector3 direction = new Vector3(Mathf.Sin(currentAngleY * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngleY * Mathf.Deg2Rad));
        Vector3 targetPosition = NPC.position - direction * fixedDistance;
        targetPosition.y = camera.transform.position.y;

        while (Vector3.Distance(camera.transform.position, targetPosition) > 0.1f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        conversationCameraPosition = camera.transform.position;
        conversationCameraRotation = camera.transform.rotation;
        camera.transform.position = targetPosition;
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
    }

    private IEnumerator ReturnCameraToConversationPosition()
    {
        while (Vector3.Distance(camera.transform.position, conversationCameraPosition) > 0.1f || Quaternion.Angle(camera.transform.rotation, conversationCameraRotation) > 0.1f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, conversationCameraPosition, movementSpeed * Time.deltaTime);
            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, conversationCameraRotation, movementSpeed * Time.deltaTime);
            yield return null;
        }

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
