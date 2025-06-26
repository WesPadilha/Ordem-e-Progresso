using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class QuestNoComplete : MonoBehaviour
{
    [Header("Referências da Missão")]
    public CreateMissions mission;   // A missão que será verificada
    public string targetID;          // O ID do alvo que será verificado
    public string dialogueParameterName = "ObjetivoCompleto"; // Nome da variável no Dialogue Editor

    // Este método pode ser chamado diretamente pelo campo "Events" no Dialogue Editor
    public void VerifyObjectiveStatusForDialogue()
    {
        if (mission == null)
        {
            Debug.LogWarning("Missão não atribuída ao QuestNoComplete.");
            ConversationManager.Instance.SetInt(dialogueParameterName, 0); // false
            return;
        }

        foreach (var objective in mission.objectives)
        {
            if (objective.targetID == targetID)
            {
                int value = objective.isCompleted ? 1 : 0;
                ConversationManager.Instance.SetInt(dialogueParameterName, value);
                Debug.Log($"[QuestNoComplete] Objetivo '{targetID}' foi {(value == 1 ? "concluído" : "não concluído")}.");
                return;
            }
        }

        Debug.LogWarning($"[QuestNoComplete] Objetivo com targetID '{targetID}' não encontrado na missão '{mission.missionName}'.");
        ConversationManager.Instance.SetInt(dialogueParameterName, 0);
    }
}
