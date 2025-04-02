using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupController : MonoBehaviour
{
    public List<PlayerProximityDetector> enemiesInGroup = new List<PlayerProximityDetector>();
    public ChangeMovimentController changeMovimentController;
    public bool groupActivated = false; // Indica se este grupo já foi ativado

    public void RegisterEnemy(PlayerProximityDetector enemy)
    {
        if (enemy != null && !enemiesInGroup.Contains(enemy))
        {
            enemiesInGroup.Add(enemy);
        }
    }

    public void ActivateGroup()
    {
        if (groupActivated) return;

        groupActivated = true;
        GlobalEnemyGroupManager.Instance.RegisterActiveGroup(this); // Registra este grupo como ativo

        if (!GlobalEnemyGroupManager.Instance.gridActivated && changeMovimentController != null)
        {
            changeMovimentController.ToggleGrid();
            GlobalEnemyGroupManager.Instance.gridActivated = true;
        }

        foreach (var enemy in enemiesInGroup)
        {
            if (enemy != null && enemy.enemyController != null)
            {
                enemy.enemyController.SetActive(true);
            }
        }
    }

    public void EnemyDestroyed(PlayerProximityDetector enemy)
    {
        if (enemy != null && enemiesInGroup.Contains(enemy))
        {
            enemiesInGroup.Remove(enemy);

            if (enemiesInGroup.Count == 0)
            {
                groupActivated = false;
                GlobalEnemyGroupManager.Instance.UnregisterGroup(this); // Notifica que este grupo foi derrotado
            }
        }
    }
}