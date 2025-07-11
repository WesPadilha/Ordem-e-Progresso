using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public enum TurnPhase { Player, Enemies }
    public TurnPhase currentPhase = TurnPhase.Player;

    public MovimentCombat playerMovement;
    private List<EnemyGroupController> activeGroups = new List<EnemyGroupController>();
    private int currentGroupIndex = 0;
    private int currentEnemyIndex = 0;

    public delegate void TurnChangedHandler(TurnPhase phase, GameObject activeCharacter);
    public event TurnChangedHandler OnTurnChanged;

    void Start()
    {
        StartPlayerTurn();
    }

    void Update()
    {
        if (currentPhase == TurnPhase.Player)
        {
            if (playerMovement.GetCurrentActionPoints() <= 0)
            {
                EndPlayerTurn();
            }
        }
        else if (currentPhase == TurnPhase.Enemies)
        {
            if (activeGroups.Count > 0 && currentGroupIndex < activeGroups.Count)
            {
                var currentGroup = activeGroups[currentGroupIndex];
                currentGroup.enemies.RemoveAll(enemy => enemy == null || enemy.gameObject == null);
                
                if (currentEnemyIndex < currentGroup.enemies.Count)
                {
                    EnemyChase currentEnemy = currentGroup.enemies[currentEnemyIndex];
                    
                    if (currentEnemy == null || currentEnemy.gameObject == null)
                    {
                        EndCurrentEnemyTurn();
                    }
                    else if (currentEnemy.GetCurrentActionPoints() <= 0 || !currentEnemy.IsChasing())
                    {
                        EndCurrentEnemyTurn();
                    }
                }
                else
                {
                    EndCurrentGroupTurn();
                }
            }
            else
            {
                EndEnemiesTurn();
            }
        }
    }

    public void RegisterEnemyGroup(EnemyGroupController group)
    {
        if (!activeGroups.Contains(group))
        {
            activeGroups.Add(group);
            Debug.Log($"Grupo {group.name} adicionado à fila de turnos");
        }
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Turno do Player terminou.");
        
        // Zera os pontos de ação do jogador
        playerMovement.SpendActionPoints((int)playerMovement.GetCurrentActionPoints());

        playerMovement.DisableMovement();
        StartEnemiesTurn();
    }

    void StartPlayerTurn()
    {
        currentPhase = TurnPhase.Player;
        Debug.Log("Turno do Player começou.");
        playerMovement.ResetActionPoints();
        OnTurnChanged?.Invoke(currentPhase, playerMovement.gameObject);
    }

    void StartEnemiesTurn()
    {
        currentPhase = TurnPhase.Enemies;
        currentGroupIndex = 0;
        currentEnemyIndex = 0;
        
        if (activeGroups.Count > 0)
        {
            StartCurrentGroupTurn();
        }
        else
        {
            Debug.Log("Nenhum grupo ativo, voltando para turno do jogador.");
            EndEnemiesTurn();
        }
    }

    void StartCurrentGroupTurn()
    {
        // Verifica se o índice é válido e se o grupo ainda existe
        if (currentGroupIndex < activeGroups.Count && activeGroups[currentGroupIndex] != null)
        {
            var currentGroup = activeGroups[currentGroupIndex];
            
            // Verifica se o GameObject do grupo ainda existe
            if (currentGroup.gameObject == null)
            {
                activeGroups.RemoveAt(currentGroupIndex);
                StartCurrentGroupTurn(); // Tenta o próximo grupo
                return;
            }

            Debug.Log($"Turno do grupo {currentGroupIndex + 1} ({currentGroup.name}) começou.");
            
            if (currentEnemyIndex < currentGroup.enemies.Count)
            {
                StartCurrentEnemyTurn();
            }
        }
        else if (currentGroupIndex < activeGroups.Count)
        {
            // Remove o grupo nulo da lista
            activeGroups.RemoveAt(currentGroupIndex);
            StartCurrentGroupTurn(); // Tenta o próximo grupo
        }
    }

    void StartCurrentEnemyTurn()
    {
        var currentGroup = activeGroups[currentGroupIndex];
        EnemyChase currentEnemy = currentGroup.enemies[currentEnemyIndex];
        
        if (currentEnemy != null && currentEnemy.gameObject != null)
        {
            Debug.Log($"Turno do Inimigo {currentEnemyIndex + 1} do grupo {currentGroupIndex + 1} começou.");
            currentEnemy.ResetActionPoints();
            currentEnemy.StartChasing();
            OnTurnChanged?.Invoke(currentPhase, currentEnemy.gameObject);
        }
        else
        {
            EndCurrentEnemyTurn();
        }
    }

    void EndCurrentEnemyTurn()
    {
        var currentGroup = activeGroups[currentGroupIndex];
        
        if (currentEnemyIndex < currentGroup.enemies.Count)
        {
            EnemyChase currentEnemy = currentGroup.enemies[currentEnemyIndex];
            if (currentEnemy != null && currentEnemy.gameObject != null)
            {
                currentEnemy.DisableMovement();
            }
        }
        
        currentEnemyIndex++;
        
        if (currentEnemyIndex < currentGroup.enemies.Count)
        {
            StartCurrentEnemyTurn();
        }
        else
        {
            CheckAllEnemiesDefeated();
        }
    }

    void EndCurrentGroupTurn()
    {
        var currentGroup = activeGroups[currentGroupIndex];
        if (currentGroup != null && currentGroup.gameObject != null)
        {
            currentGroup.enemies.RemoveAll(enemy => enemy == null || enemy.gameObject == null);
            
            if (currentGroup.enemies.Count == 0)
            {
                activeGroups.RemoveAt(currentGroupIndex);
                CheckAllEnemiesDefeated();
                return;
            }
        }

        currentGroupIndex++;
        currentEnemyIndex = 0;
        
        if (currentGroupIndex < activeGroups.Count)
        {
            StartCurrentGroupTurn();
        }
        else
        {
            CheckAllEnemiesDefeated();
        }
    }

    void EndEnemiesTurn()
    {
        CheckAllEnemiesDefeated();
        OnTurnChanged?.Invoke(TurnPhase.Player, null); // Clear indicator
        StartPlayerTurn();
    }

    void CheckAllEnemiesDefeated()
    {
        // Remove todos os grupos nulos ou destruídos
        activeGroups.RemoveAll(group => group == null || group.gameObject == null || group.enemies.Count == 0);

        if (activeGroups.Count == 0)
        {
            var playerSwitcher = FindObjectOfType<PlayerControllerSwitcher>();
            if (playerSwitcher != null)
            {
                playerSwitcher.SwitchToMainController();
                Debug.Log("Todos os inimigos foram derrotados. Voltando para modo de exploração.");
            }
        }
    }
}