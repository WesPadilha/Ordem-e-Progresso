using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleController : MonoBehaviour
{
    public MainController mainController;
    public MovimentCombat movimentCombat;

    private void Start()
    {
        mainController.enabled = true;
        movimentCombat.enabled = false;
    }

    public void Toggle()
    {
        // Resetar os pontos de ação ANTES da troca
        if (!movimentCombat.enabled)
        {
            movimentCombat.ResetActionPoints();
        }

        mainController.enabled = !mainController.enabled;
        movimentCombat.enabled = !movimentCombat.enabled;
    }
}
