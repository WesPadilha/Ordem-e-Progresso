using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupController : MonoBehaviour
{
    public List<EnemyChase> enemies = new List<EnemyChase>();
    private bool isChasing = false;
    private PlayerControllerSwitcher controllerSwitcher;
    private TurnManager turnManager;

    [Header("NPC Components")]
    public DialogNPC dialogNPCScript;
    public NPCOrientationController npcOrientationControllerScript;

    [Header("Enemy Components")]
    public DetectionImage detectionImageScript;
    public PlayerProximityDetector proximityDetectorScript;

    void Start()
    {
        enemies.AddRange(GetComponentsInChildren<EnemyChase>(true)); // Inclui inativos
        controllerSwitcher = FindObjectOfType<PlayerControllerSwitcher>();
        turnManager = FindObjectOfType<TurnManager>();

        if (controllerSwitcher == null)
        {
            Debug.LogWarning("PlayerControllerSwitcher não encontrado na cena");
        }

        if (turnManager == null)
        {
            Debug.LogWarning("TurnManager não encontrado na cena");
        }
    }

    public void StartGroupChase()
    {
        if (!isChasing)
        {
            isChasing = true;
            turnManager.RegisterEnemyGroup(this);

            // Desativa scripts de NPC para todos os inimigos do grupo
            if (dialogNPCScript != null)
            {
                dialogNPCScript.enabled = false;
            }

            if (npcOrientationControllerScript != null)
            {
                npcOrientationControllerScript.enabled = false;
            }

            // Ativa componentes de inimigo para todos do grupo
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.enabled = true;
                    enemy.ResetActionPoints();

                    // Ativa componentes adicionais se existirem
                    if (detectionImageScript != null)
                    {
                        detectionImageScript.enabled = true;
                    }

                    if (proximityDetectorScript != null)
                    {
                        proximityDetectorScript.enabled = true;
                    }
                }
            }

            if (controllerSwitcher != null)
            {
                controllerSwitcher.SwitchControllers();
            }

            Debug.Log($"Grupo {gameObject.name} começou a perseguir o player! Todos os inimigos ativados.");
        }
    }

    public void RemoveEnemy(EnemyChase enemy)
    {
        if (enemy == null) return;

        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Debug.Log($"Inimigo removido do grupo {name}. Inimigos restantes: {enemies.Count}");
            
            if (enemies.Count == 0)
            {
                isChasing = false;
                
                bool anyEnemiesLeft = false;
                var allGroups = FindObjectsOfType<EnemyGroupController>();
                foreach (var group in allGroups)
                {
                    if (group != null && group != this && group.enemies.Count > 0)
                    {
                        anyEnemiesLeft = true;
                        break;
                    }
                }
                
                if (!anyEnemiesLeft)
                {
                    var controllerSwitcher = FindObjectOfType<PlayerControllerSwitcher>();
                    if (controllerSwitcher != null)
                    {
                        controllerSwitcher.SwitchToMainController();
                        Debug.Log("Todos os inimigos foram derrotados. Voltando para modo de exploração.");
                    }
                }
            }
        }
    }

    public bool IsGroupChasing()
    {
        return isChasing;
    }

    // Método para adicionar um inimigo ao grupo dinamicamente
    public void AddEnemy(EnemyChase newEnemy)
    {
        if (!enemies.Contains(newEnemy))
        {
            enemies.Add(newEnemy);
            Debug.Log($"Novo inimigo adicionado ao grupo {name}. Total: {enemies.Count}");

            // Se o grupo já estiver em perseguição, ativa o novo inimigo imediatamente
            if (isChasing)
            {
                newEnemy.enabled = true;
                newEnemy.ResetActionPoints();
            }
        }
    }
}