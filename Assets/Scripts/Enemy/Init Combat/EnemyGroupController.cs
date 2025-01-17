using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupController : MonoBehaviour
{
    public List<PlayerProximityDetector> enemiesInGroup = new List<PlayerProximityDetector>();
    public ChangeMovimentController changeMovimentController; // Referência ao controlador de movimentação

    public void RegisterEnemy(PlayerProximityDetector enemy)
    {
        if (enemy != null && !enemiesInGroup.Contains(enemy))
        {
            enemiesInGroup.Add(enemy);
        }
    }

    public void EnemyDestroyed(PlayerProximityDetector enemy)
    {
        if (enemy != null && enemiesInGroup.Contains(enemy))
        {
            enemiesInGroup.Remove(enemy);

            // Verifica se todos os inimigos foram destruídos
            if (enemiesInGroup.Count == 0 && changeMovimentController != null)
            {
                changeMovimentController.ToggleGrid();
            }
        }
    }

    public void ActivateAllEnemies()
    {
        foreach (var enemy in enemiesInGroup)
        {
            if (enemy != null)
            {
                enemy.SetHasTriggered(true);
            }
        }
    }
}
