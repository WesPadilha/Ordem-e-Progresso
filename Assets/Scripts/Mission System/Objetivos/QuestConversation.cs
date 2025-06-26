using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestConversation : MonoBehaviour
{
    public string missionID; // ID da missão relacionada
    public string targetID;  // ID do objeto-alvo

    // Referência ao MissionManager para acessar missões
    public MissionManager missionManager;

    public void CompleteInteractWithNPCObjective()
    {
        if (missionManager == null)
        {
            Debug.LogWarning("MissionManager não atribuído no QuestConversation.");
            return;
        }

        CreateMissions mission = missionManager.acceptedMissions.Find(m => m.missionID == missionID);

        if (mission != null && mission.isActive && !mission.isCompleted)
        {
            mission.ReportNPCInteracted(targetID); // Método correto para InteractWithNPC

            Debug.Log($"Objetivo InteractWithNPC concluído na missão: {mission.missionName}");
        }
        else
        {
            Debug.LogWarning("Missão não encontrada, não ativa ou já concluída.");
        }
    }
}
