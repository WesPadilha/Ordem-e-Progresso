using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSwitcher : MonoBehaviour
{
    public MainController mainController;
    public MovimentCombat movimentCombat;

    void Start()
    {
        // Opcional: garantir estado inicial
        mainController.enabled = true;
        movimentCombat.enabled = false;
    }

    public void SwitchControllers()
    {
        // Para movimentos ativos
        mainController.StopMovement();
        movimentCombat.StopMovement();

        mainController.enabled = false;
        movimentCombat.enabled = true;
    }

    public void SwitchToMainController()
    {
        movimentCombat.StopMovement();
        mainController.StopMovement();

        mainController.enabled = true;
        movimentCombat.enabled = false;
    }
}
