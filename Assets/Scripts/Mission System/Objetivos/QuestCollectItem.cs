using UnityEngine;

public class QuestCollectItem : MonoBehaviour
{
    public string itemID; // ID do item (deve corresponder ao targetID no MissionObjective)

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem colidiu é o jogador
        if (other.CompareTag("Player"))
        {
            // Encontra o MissionManager na cena
            MissionManager missionManager = FindObjectOfType<MissionManager>();
            if (missionManager != null)
            {
                // Notifica todas as missões aceitas sobre o item coletado
                foreach (var mission in missionManager.acceptedMissions)
                {
                    mission.ReportItemCollected(itemID);
                }
            }
        }
    }
}