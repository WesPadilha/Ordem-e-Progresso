using System.Linq;
using UnityEngine;

public class CombatStatusChecker : MonoBehaviour
{
    public PlayerControllerSwitcher playerController;

    public bool IsInCombat()
    {
        bool isPlayerInCombatMode = !playerController.mainController.enabled && playerController.movimentCombat.enabled;
        bool areEnemiesActive = AreEnemiesChasing();

        return isPlayerInCombatMode && areEnemiesActive;
    }
    
    public bool CanShootOutsideCombat()
    {
        // Verifica se o player está fora de combate e tem uma arma equipada
        WeaponController weapon = FindObjectOfType<WeaponController>();
        return !IsInCombat() && weapon != null && weapon.CanShootAtEnemy();
    }

    public bool IsPlayerInCombatMode()
    {
        return !playerController.mainController.enabled && playerController.movimentCombat.enabled;
    }

    public bool AreEnemiesChasing()
    {
        EnemyGroupController[] groups = FindObjectsOfType<EnemyGroupController>();

        foreach (var group in groups)
        {
            if (group.IsGroupChasing() && group.enemies.Count > 0)
                return true;
        }

        return false;
    }
}
