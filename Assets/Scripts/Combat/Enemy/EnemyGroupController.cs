using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupController : MonoBehaviour
{
    public List<EnemyChase> enemies = new List<EnemyChase>();
    private bool isChasing = false;
    private PlayerControllerSwitcher controllerSwitcher;
    private TurnManager turnManager;

    void Start()
    {
        enemies.AddRange(GetComponentsInChildren<EnemyChase>());
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

            foreach (var enemy in enemies)
            {
                enemy.ResetActionPoints();
            }

            if (controllerSwitcher != null)
            {
                controllerSwitcher.SwitchControllers();
            }

            Debug.Log($"Grupo {gameObject.name} começou a perseguir o player!");
        }
    }

    public void RemoveEnemy(EnemyChase enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Debug.Log($"Inimigo removido do grupo {name}. Inimigos restantes: {enemies.Count}");
            
            // Verifica se o grupo foi totalmente eliminado
            if (enemies.Count == 0)
            {
                // Verifica se há outros grupos ativos
                bool anyEnemiesLeft = false;
                foreach (var group in FindObjectsOfType<EnemyGroupController>())
                {
                    if (group.enemies.Count > 0)
                    {
                        anyEnemiesLeft = true;
                        break;
                    }
                }
                
                // Se não há mais inimigos em nenhum grupo, volta para o modo de exploração
                if (!anyEnemiesLeft && controllerSwitcher != null)
                {
                    controllerSwitcher.SwitchToMainController();
                    Debug.Log("Todos os inimigos foram derrotados. Voltando para modo de exploração.");
                }
                
                Destroy(gameObject); // Opcional: destruir o grupo quando vazio
            }
        }
    }

    public bool IsGroupChasing()
    {
        return isChasing;
    }
}