using System.Collections;
using UnityEngine;

public class NoEnemy : MonoBehaviour
{
    [Header("Script References")]
    public DialogNPC dialogNPCScript;
    public NPCOrientationController npcOrientationControllerScript;
    
    [Header("Enemy Components")]
    public EnemyChase enemyChaseScript;
    public DetectionImage detectionImageScript;
    public PlayerProximityDetector proximityDetectorScript;

    [Header("Combat References")]
    public PlayerControllerSwitcher playerControllerSwitcher;
    public TurnManager turnManager;
    public EnemyGroupController enemyGroupController;

    // Método público que pode ser chamado por eventos
    public void ActivateEnemyMode()
    {
        StartCoroutine(ActivateEnemyModeWithDelay());
    }

    private IEnumerator ActivateEnemyModeWithDelay()
    {
        // Espera 1 segundo antes de executar
        yield return new WaitForSeconds(1f);

        // Desativa scripts de NPC
        if (dialogNPCScript != null)
        {
            dialogNPCScript.enabled = false;
        }
        else
        {
            Debug.LogWarning("DialogNPC script reference not set in NoEnemy");
        }

        if (npcOrientationControllerScript != null)
        {
            npcOrientationControllerScript.enabled = false;
        }
        else
        {
            Debug.LogWarning("NPCOrientationController script reference not set in NoEnemy");
        }

        // Ativa scripts de inimigo
        if (enemyChaseScript != null)
        {
            enemyChaseScript.enabled = true;
            enemyChaseScript.ResetActionPoints();
        }
        else
        {
            Debug.LogWarning("EnemyChase script reference not set in NoEnemy");
        }

        if (detectionImageScript != null)
        {
            detectionImageScript.enabled = true;
        }
        else
        {
            Debug.LogWarning("DetectionImage script reference not set in NoEnemy");
        }

        if (proximityDetectorScript != null)
        {
            proximityDetectorScript.enabled = true;
        }
        else
        {
            Debug.LogWarning("PlayerProximityDetector script reference not set in NoEnemy");
        }

        // Força o estado de combate
        ForceCombatState();

        Debug.Log("Enemy mode activated after delay - Combat started forcefully");
    }

    private void ForceCombatState()
    {
        // Ativa o modo de combate do player
        if (playerControllerSwitcher != null)
        {
            playerControllerSwitcher.SwitchControllers();
        }
        else
        {
            Debug.LogWarning("PlayerControllerSwitcher reference not set in NoEnemy");
        }

        // Registra o grupo de inimigos no TurnManager
        if (turnManager != null && enemyGroupController != null)
        {
            if (!enemyGroupController.IsGroupChasing())
            {
                enemyGroupController.StartGroupChase();
            }
            turnManager.RegisterEnemyGroup(enemyGroupController);
        }
        else
        {
            Debug.LogWarning("TurnManager or EnemyGroupController reference not set in NoEnemy");
        }

        // Ativa o inimigo no grupo
        if (enemyGroupController != null && !enemyGroupController.enemies.Contains(enemyChaseScript))
        {
            enemyGroupController.enemies.Add(enemyChaseScript);
        }
    }
}