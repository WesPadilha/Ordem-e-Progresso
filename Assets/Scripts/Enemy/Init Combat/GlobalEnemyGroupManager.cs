using System.Collections.Generic;
using UnityEngine;

public class GlobalEnemyGroupManager : MonoBehaviour
{
    public static GlobalEnemyGroupManager Instance;

    public bool gridActivated = false; // Indica se o grid está ativo
    private List<EnemyGroupController> activeGroups = new List<EnemyGroupController>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Registra um grupo ativo
    public void RegisterActiveGroup(EnemyGroupController group)
    {
        if (!activeGroups.Contains(group))
        {
            activeGroups.Add(group);
        }
    }

    // Remove um grupo e verifica se todos foram destruídos
    public void UnregisterGroup(EnemyGroupController group)
    {
        if (activeGroups.Contains(group))
        {
            activeGroups.Remove(group);
            CheckAllGroupsDefeated();
        }
    }

    // Verifica se todos os grupos foram derrotados
    private void CheckAllGroupsDefeated()
    {
        if (activeGroups.Count == 0 && gridActivated)
        {
            ChangeMovimentController changeController = FindObjectOfType<ChangeMovimentController>();
            if (changeController != null)
            {
                changeController.ToggleGrid();
                gridActivated = false; // Reseta o estado do grid
            }
        }
    }
}