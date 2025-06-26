using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissionType
{
    KillEnemy,
    CollectItem,
    InteractObject,
    InteractWithNPC,
    ExploreArea
}

[System.Serializable]
public class MissionObjective
{
    public MissionType type;
    public bool isCompleted;

    public string targetID;            // ID genérico (inimigo, item, objeto, área)
    public int requiredAmount = 1;     // Quantidade necessária
    public int currentAmount = 0;

    [TextArea(5, 10)]
    public string description; 
    public string moreDescription;        // Descrição do objetivo
}

[CreateAssetMenu(fileName = "New Mission", menuName = "Mission/Create Mission")]
public class CreateMissions : ScriptableObject
{
    [Header("Identificação")]
    public string missionID;           // Novo campo: ID único da missão
    public string missionName;

    [TextArea(5, 10)]
    public string description;

    [Header("Recompensas")]
    public int rewardExperience;
    public int rewardMoney;
    public int rewardItemID;

    [Header("Estado da Missão")]
    public bool isActive;
    public bool isCompleted;

    [Header("Objetivos")]
    public List<MissionObjective> objectives = new List<MissionObjective>();

    [Header("Referências")]
    public InventoryObject playerInventory;

    public void CheckMissionCompletion()
    {
        foreach (var objective in objectives)
        {
            if (!objective.isCompleted)
            {
                isCompleted = false;
                return;
            }
        }

        isCompleted = true;

        // Adiciona experiência se houver recompensa
        if (ExperienceManager.Instance != null && rewardExperience > 0)
        {
            ExperienceManager.Instance.AddExperience(rewardExperience);
        }

        // Adiciona dinheiro ao inventário do jogador se houver recompensa
        if (rewardMoney > 0 && playerInventory != null)
        {
            playerInventory.Money += rewardMoney;
            Debug.Log($"Recompensa de {rewardMoney} dinheiro adicionada ao inventário");
        }
    }

    public void CompleteObjective(MissionType type)
    {
        foreach (var objective in objectives)
        {
            if (objective.type == type && !objective.isCompleted)
            {
                objective.isCompleted = true;
                break;
            }
        }

        CheckMissionCompletion();
    }

    // Versões com string para cada tipo
    public void ReportEnemyKilled(string enemyID)
    {
        UpdateObjectiveProgress(MissionType.KillEnemy, enemyID);
    }

    public void ReportItemCollected(string itemID)
    {
        UpdateObjectiveProgress(MissionType.CollectItem, itemID);
    }

    public void ReportObjectInteracted(string objectID)
    {
        UpdateObjectiveProgress(MissionType.InteractObject, objectID);
    }

    public void ReportAreaExplored(string areaID)
    {
        UpdateObjectiveProgress(MissionType.ExploreArea, areaID);
    }

    public void ReportNPCInteracted(string npcID)
    {
        UpdateObjectiveProgress(MissionType.InteractWithNPC, npcID);
    }

    // Função genérica com string
    private void UpdateObjectiveProgress(MissionType type, string targetID)
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            var objective = objectives[i];

            if (!objective.isCompleted)
            {
                // Só permite progresso no próximo objetivo pendente
                if (objective.type == type && objective.targetID == targetID)
                {
                    objective.currentAmount++;

                    if (objective.currentAmount >= objective.requiredAmount)
                    {
                        objective.isCompleted = true;
                        Debug.Log($"Objetivo {i + 1} concluído: {objective.type}");
                    }
                }
                break; // Encerra após tentar completar apenas o próximo objetivo pendente
            }
        }

        CheckMissionCompletion();
    }

    public void ResetObjectives()
    {
        isActive = false;
        isCompleted = false;

        foreach (var objective in objectives)
        {
            objective.isCompleted = false;
            objective.currentAmount = 0;
        }
    }
}
