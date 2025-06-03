using UnityEngine;

public class CombatStateManager : MonoBehaviour
{
    public PlayerControllerSwitcher playerControllerSwitcher;
    public MovementUI movementUI;
    public TurnManager turnManager;

    void Update()
    {
        UpdateCombatStateUI();
    }

    void UpdateCombatStateUI()
    {
        bool playerInCombat = IsPlayerInCombat();

        if (playerInCombat)
        {
            // Mostra os pontos de ação do player
            movementUI.UpdateActionPoints((int)turnManager.playerMovement.GetCurrentActionPoints());
        }
        else
        {
            // Mostra "--" quando não está em combate
            movementUI.ShowNoCombat();
        }
    }

    public bool IsPlayerInCombat()
    {
        // Verifica se o player está no modo combate e se há inimigos ativos
        bool playerCombatMode = !playerControllerSwitcher.mainController.enabled && playerControllerSwitcher.movimentCombat.enabled;

        bool enemiesActive = false;
        EnemyGroupController[] groups = FindObjectsOfType<EnemyGroupController>();
        foreach (var group in groups)
        {
            if (group != null && group.IsGroupChasing())
            {
                enemiesActive = true;
                break;
            }
        }

        return playerCombatMode && enemiesActive;
    }
}
