using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDefeatedHandler : MonoBehaviour
{
    [Header("Configurações de Inimigos")]
    [Tooltip("Lista de inimigos que serão deletados quando o evento for acionado")]
    public List<GameObject> enemiesToDelete = new List<GameObject>();

    [Header("Configurações de XP")]
    [Tooltip("Quantidade de XP que será concedida ao jogador")]
    public int xpToAdd = 50;

    [Header("Configurações de Missão")]
    [Tooltip("Referência à missão que será atualizada")]
    public CreateMissions missionToUpdate;

    [Header("Referências")]
    [Tooltip("Referência ao CharacterData do jogador (opcional)")]
    public CharacterData playerCharacterData;

    private void OnEnable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange += HandleExperienceEvent;
        }
    }

    private void OnDisable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange -= HandleExperienceEvent;
        }
    }

    public void OnEnemiesDefeatedEvent()
    {
        StartCoroutine(HandleEnemyDefeatWithDelay());
    }

    private IEnumerator HandleEnemyDefeatWithDelay()
    {
        yield return new WaitForSeconds(1f);
        
        DeleteEnemies();
        AddExperience();
        UpdateMissionProgress();
    }

    private void DeleteEnemies()
    {
        foreach (GameObject enemy in enemiesToDelete)
        {
            if (enemy != null)
            {
                // Verifica se o inimigo tem um QuestKillEnemy para obter o enemyID
                var questComponent = enemy.GetComponent<QuestKillEnemy>();
                if (questComponent != null && missionToUpdate != null)
                {
                    // Reporta cada inimigo morto para a missão
                    missionToUpdate.ReportEnemyKilled(questComponent.enemyID);
                }

                Destroy(enemy);
            }
        }

        enemiesToDelete.Clear();
    }

    private void AddExperience()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(xpToAdd);
        }
        else if (playerCharacterData != null)
        {
            playerCharacterData.AddExperience(xpToAdd);
        }
        else
        {
            Debug.LogWarning("Nenhum sistema de XP encontrado para adicionar experiência.");
        }
    }

    private void UpdateMissionProgress()
    {
        if (missionToUpdate != null)
        {
            // Verifica se todos os objetivos foram completados
            missionToUpdate.CheckMissionCompletion();
            
            if (missionToUpdate.isCompleted)
            {
                Debug.Log($"Missão {missionToUpdate.missionName} completada!");
            }
        }
    }

    private void HandleExperienceEvent(int amount)
    {
        Debug.Log($"Evento de XP recebido: {amount} XP");
    }
}