using UnityEngine;

public class QuestInteractObject : MonoBehaviour
{
    public string missionID; // ID da missão relacionada
    public string targetID;  // ID do objeto-alvo

    public float interactionDistance = 3f;

    private Transform playerTransform;
    private MissionManager missionManager;

    private bool isWaitingForInteraction = false;
    private static QuestInteractObject currentlySelectedObject;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player não encontrado na cena com tag 'Player'.");
        }

        missionManager = FindObjectOfType<MissionManager>();
        if (missionManager == null)
        {
            Debug.LogWarning("MissionManager não encontrado na cena.");
        }
    }

    void Update()
    {
        if (playerTransform == null || missionManager == null) return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);

        // Executa interação se já estava esperando e o player se aproximou
        if (isWaitingForInteraction && distance <= interactionDistance)
        {
            CompleteInteractionObjective();
            isWaitingForInteraction = false;
            currentlySelectedObject = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOverObject())
            {
                // Se clicou neste objeto
                float clickDistance = Vector3.Distance(playerTransform.position, transform.position);
                if (clickDistance <= interactionDistance)
                {
                    // Interage imediatamente
                    CompleteInteractionObjective();
                }
                else
                {
                    // Marca como aguardando interação
                    if (currentlySelectedObject != null && currentlySelectedObject != this)
                    {
                        currentlySelectedObject.CancelSelection();
                    }

                    isWaitingForInteraction = true;
                    currentlySelectedObject = this;
                    Debug.Log("Objeto selecionado, aproxime-se para interagir.");
                }
            }
            else
            {
                // Clicou fora deste objeto, cancelar seleção
                if (currentlySelectedObject != null)
                {
                    currentlySelectedObject.CancelSelection();
                    currentlySelectedObject = null;
                }
            }
        }
    }

    private bool IsMouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    private void CompleteInteractionObjective()
    {
        var mission = missionManager.acceptedMissions.Find(m => m.missionID == missionID);
        if (mission != null && mission.isActive)
        {
            mission.ReportObjectInteracted(targetID);
            Debug.Log($"Interação completada na missão {mission.missionName}");
        }
        else
        {
            Debug.LogWarning($"Missão com ID {missionID} não está ativa ou não foi aceita.");
        }
    }

    public void CancelSelection()
    {
        isWaitingForInteraction = false;
        Debug.Log("Interação cancelada.");
    }
}
